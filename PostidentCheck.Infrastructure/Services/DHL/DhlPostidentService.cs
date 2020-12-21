using FluentValidation;
using KeePass.Models;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.DHL;
using Postident.Application.DHL.Interfaces;
using Postident.Core.Entities;
using Postident.Infrastructure.Interfaces.DHL;
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
    public class DhlPostidentService : ApiServiceBase<DhlResponseDto, DataPackReadModel>, IDhlApiService
    {
        private const string ServiceName = "DHL PostIdent";

        private readonly ILogger<DhlPostidentService> _logger;
        private readonly IDhlSettings _configuration;

        public DhlPostidentService(IHttpClientFactory httpFactory,
            ICarrierApiServiceResponseDeserializer<DhlResponseDto> serializer,
            IValidator<DataPackReadModel> parcelValidator,
            ILogger<DhlPostidentService> logger,
            IDhlSettings configuration)
        : base(ServiceName, serializer, parcelValidator, logger)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Client = httpFactory.CreateClient("DHL");

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
                _logger.LogError(ex, "{0}: Exception caught when trying to authenticate http client", ServiceName);
                return false;
            }
        }

        protected override async Task<IEnumerable<HttpRequestMessage>> GenerateRequestsFrom(IEnumerable<DataPackReadModel> dataPacks, CancellationToken ct)
        {
            var xmlSecret = await RetrieveXmlAuthorizationData(ct);
            throw new NotImplementedException();
            //var output = parcels
            //    .Batch(_configuration.MaxParcelNumbersInQuery)
            //    .Select(batch => CreateCombinedRequestFrom(batch, xmlSecret))
            //    .ToList();

            //_logger.LogInformation("{0}: Generating {1} requests from {2} parcels.", ServiceName, output.Count, parcels.Count());

            //return await Task.FromResult(output);
        }

        protected override Task<IEnumerable<HttpResponseMessage>> GetOnlyDeserializableResponses(IEnumerable<HttpResponseMessage> responses)
        {
            // Internal service errors from DHL also can be deserialized
            return Task.FromResult(responses.Where(r => r.IsSuccessStatusCode || r.StatusCode == HttpStatusCode.InternalServerError));
        }

        private Task<Secret> RetrieveXmlAuthorizationData(CancellationToken ct) => _configuration.XmlSecret(ct);
    }
}