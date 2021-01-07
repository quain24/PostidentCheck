using KeePass.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Postident.Application.Common.Extensions;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Application.DHL;
using Postident.Application.DHL.Interfaces;
using Postident.Infrastructure.Interfaces.DHL;
using SharedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services.DHL
{
    public class DhlOnlineValidationService : ApiServiceBase<DhlMainResponseDto, DataPack>, IDhlApiService
    {
        private const string ServiceName = "DHL online validation";

        private readonly IValidationRequestXmlBuilderFactory _xmlRequestBuilderFactory;
        private readonly ILogger<DhlOnlineValidationService> _logger;
        private readonly IDhlSettings _configuration;

        public DhlOnlineValidationService(IHttpClientFactory httpFactory,
            ICarrierApiServiceResponseDeserializer<DhlMainResponseDto> serializer,
            IValidationRequestXmlBuilderFactory xmlRequestBuilderFactory,
            ILogger<DhlOnlineValidationService> logger,
            IDhlSettings configuration)
        : base(ServiceName, serializer, logger)
        {
            _xmlRequestBuilderFactory = xmlRequestBuilderFactory ?? throw new ArgumentNullException(nameof(xmlRequestBuilderFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;

            Client = httpFactory.CreateClient("DHL PostIdent");
            QueriesPerSecond = _configuration.MaxQueriesPerSecond;
        }

        protected override HttpClient Client { get; }
        protected override int QueriesPerSecond { get; }
        private Secret XmlSecret { get; set; }

        protected override async Task<bool> AuthorizeClient(CancellationToken token)
        {
            try
            {
                var secret = await _configuration.Secret(token).ConfigureAwait(false);
                XmlSecret = await _configuration.XmlSecret(token).ConfigureAwait(false);

                var byteArray = Encoding.ASCII.GetBytes(string.Join(':', secret.Username, secret.Password));
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                return await ValidateLoginDataOnline(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: Exception caught when trying to create authentication for http client", ServiceName);
                return false;
            }
        }

        /// <summary>
        /// This method will validate logins and passwords online.<br/>
        /// Http basic auth is validated against response to a 'dummy' request - <see cref="HttpStatusCode.Unauthorized"/> will be treated as authorization failure.<br/>
        /// Xml login and password are validated by deserialization of response for 'dummy' request - return code '118' will be threaded as failure - as stated in API docs.<br/>
        /// This methods purpose is to block any requests if authorization failed, so the API wont lock main account by mistake.
        /// </summary>
        private async Task<bool> ValidateLoginDataOnline(CancellationToken ct)
        {
            _logger?.LogInformation("{0}: Checking xml login and password validity online...", ServiceName);
            var testContent = _xmlRequestBuilderFactory
                .CreateInstance()
                .SetUpAuthorization(XmlSecret.Username, XmlSecret.Password)
                .AddNewShipment("test", new Address { City = "test", CountryCode = "de", Name = "test", Street = "test" })
                .BuildShipment()
                .Build();
            var context = new Context().WithLogger(_logger);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty) { Content = new StringContent(testContent) };
            request.SetPolicyExecutionContext(context);

            var response = await Client.SendAsync(request, ct).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger?.LogError("{0}: Online service informs that supplied login or password is invalid!", ServiceName);
                return false;
            }

            var dto = (await SerializeResponsesToDtos(new[] { response }, ct)).FirstOrDefault();

            if (!int.TryParse(dto?.MainFaultCode, out var errorCode) || (errorCode >= 110 && errorCode <= 119))
            {
                _logger?.LogError("{0}: Online service informs that supplied XML login or password is invalid! Error code: {1} | Error message: {2}" +
                                  " | Check data before retrying - DHL will lock account if there are to many invalid tries", ServiceName, errorCode, dto?.MainFaultText);
                return false;
            }

            return true;
        }

        protected override async Task<IEnumerable<HttpRequestMessage>> GenerateRequestsFrom(IEnumerable<DataPack> dataPacks, CancellationToken ct)
        {
            var output = dataPacks
                .Batch(_configuration.MaxValidationsInQuery)
                .Select(batch => CreateCombinedRequestFrom(batch))
                .ToList();

            _logger.LogInformation("{0}: Generating {1} requests from {2} data packs.", ServiceName, output.Count, dataPacks.Count());
            return await Task.FromResult(output).ConfigureAwait(false);
        }

        private HttpRequestMessage CreateCombinedRequestFrom(IEnumerable<DataPack> dataPacks)
        {
            var builder = _xmlRequestBuilderFactory.CreateInstance().SetUpAuthorization(XmlSecret.Username, XmlSecret.Password);
            dataPacks.ToList().ForEach(d => builder.AddNewShipment(d.Id, d.Address).BuildShipment());

            return new HttpRequestMessage(HttpMethod.Post, string.Empty)
            {
                Content = new StringContent(builder.Build())
            };
        }

        protected override Task<IEnumerable<HttpResponseMessage>> GetOnlyDeserializableResponses(IEnumerable<HttpResponseMessage> responses)
        {
            // Internal service errors from DHL also can be deserialized
            return Task.FromResult(responses.Where(r => r.IsSuccessStatusCode || r.StatusCode == HttpStatusCode.InternalServerError));
        }
    }
}