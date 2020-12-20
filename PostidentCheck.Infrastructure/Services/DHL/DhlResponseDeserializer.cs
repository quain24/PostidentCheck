using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.DHL;

namespace Postident.Infrastructure.Services.DHL
{
    /// <summary>
    /// Deserializes <see cref="HttpResponseMessage"/> from DHL api into <see cref="DhlResponseDto"/>
    /// If deserialization is impossible, returns <see lang="null"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when tries to deserialize <see lang="null"/></exception>
    public class DhlResponseDeserializer : ICarrierApiServiceResponseDeserializer<DhlResponseDto>
    {
        private readonly string _carrierName;
        private readonly ILogger<DhlResponseDeserializer> _logger;

        public DhlResponseDeserializer(string carrierName, ILogger<DhlResponseDeserializer> logger)
        {
            _carrierName = carrierName;
            _logger = logger;
        }

        public async Task<DhlResponseDto> Deserialize(HttpResponseMessage message)
        {
            try
            {
                var document = XDocument.Load(await message.Content.ReadAsStreamAsync());
                var serializer = new XmlSerializer(typeof(DhlResponseDto));

                using var reader = document.CreateReader();
                return serializer.Deserialize(reader) as DhlResponseDto;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is XmlException)
            {
                _logger?.LogError(
                    "{0}: Could not deserialize {0} api response - XML error.\nException: {1}",
                    _carrierName, ex);
                return null;
            }
        }
    }
}