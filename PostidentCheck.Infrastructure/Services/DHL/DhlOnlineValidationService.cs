using KeePass.Models;
using Microsoft.Extensions.Logging;
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
using Postident.Infrastructure.Interfaces;

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
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Client = httpFactory.CreateClient("DHL PostIdent");
            QueriesPerSecond = _configuration.MaxQueriesPerSecond;
        }

        protected override HttpClient Client { get; }
        protected override int QueriesPerSecond { get; }

        protected override async Task<bool> AuthorizeClient(CancellationToken token)
        {
            try
            {
                var secret = await _configuration.Secret(token).ConfigureAwait(false);

                var byteArray = Encoding.ASCII.GetBytes(string.Join(':', secret.Username, secret.Password));
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: Exception caught when trying to create authentication for http client", ServiceName);
                return false;
            }
        }

        protected override async Task<IEnumerable<HttpRequestMessage>> GenerateRequestsFrom(IEnumerable<DataPack> dataPacks, CancellationToken ct)
        {
            var xmlSecret = await RetrieveXmlAuthorizationData(ct).ConfigureAwait(false);

            var output = dataPacks
                .Batch(_configuration.MaxValidationsInQuery)
                .Select(batch => CreateCombinedRequestFrom(batch, xmlSecret))
                .ToList();

            _logger.LogInformation("{0}: Generating {1} requests from {2} data packs.", ServiceName, output.Count, dataPacks.Count());

            return await Task.FromResult(output).ConfigureAwait(false);
        }

        private HttpRequestMessage CreateCombinedRequestFrom(IEnumerable<DataPack> dataPacks, Secret secret)
        {
            var builder = _xmlRequestBuilderFactory.CreateInstance().SetUpAuthorization(secret);
            dataPacks.ToList().ForEach(d =>
            {
                builder.AddNewShipment()
                    .SetUpId(d.Id)
                    .SetUpReceiverData(d.Address)
                    .BuildShipment();
            });

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

        private Task<Secret> RetrieveXmlAuthorizationData(CancellationToken ct) => _configuration.XmlSecret(ct);
    }
}