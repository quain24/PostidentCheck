using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.DHL;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Postident.Infrastructure.Services.DHL
{
    /// <summary>
    /// Deserializes <see cref="HttpResponseMessage"/> from DHL api into <see cref="DhlResponseDto"/>
    /// If deserialization is impossible, returns <see lang="null"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when tries to deserialize <see lang="null"/></exception>
    public class DhlResponseDeserializer : ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>
    {
        private readonly string _carrierName;
        private readonly ILogger<DhlResponseDeserializer> _logger;

        public DhlResponseDeserializer(string carrierName, ILogger<DhlResponseDeserializer> logger)
        {
            _carrierName = carrierName;
            _logger = logger;
        }

        /// <summary>
        /// Tries to deserialize a response from DHL (multiple if available).
        /// If responses contains errors, those also will be deserialized if possible.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>deserialized <see cref="DhlMainResponseDto"/> or <see langword="null"/> if deserialization was not possible</returns>
        public async Task<DhlMainResponseDto> Deserialize(HttpResponseMessage message)
        {
            try
            {
                var document = XDocument.Parse(await message?.Content?.ReadAsStringAsync() ?? string.Empty);

                RemoveNamespacesFrom(document);

                return MajorFaultInfoIn(document) ?
                    DeserializeMajorFaultInfo(document) :
                    DeserializeInfo(document);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is XmlException)
            {
                _logger?.LogError(ex, "{0}: Could not deserialize api response.", _carrierName);
                return null;
            }
        }

        private static void RemoveNamespacesFrom(XDocument document)
        {
            document
                .Elements()
                .ToList()
                .ForEach(e => e.Attributes().Where(x => x.IsNamespaceDeclaration).Remove());

            foreach (var elem in document.Descendants())
                elem.Name = elem.Name.LocalName;
        }

        private static bool MajorFaultInfoIn(XDocument document) =>
            document.Root?.Element("Body")?.Elements("Fault").Any() ?? false;

        /// <summary>
        /// This deserialization mechanism is used when response transmits errors mostly about malformed xml major fault - no sub-responses are available
        /// </summary>
        /// <param name="document"></param>
        private static DhlMainResponseDto DeserializeMajorFaultInfo(XDocument document)
        {
            var faultInfo = document?.Root?.Element("Body")?.Element("Fault");

            return faultInfo is null ?
                new DhlMainResponseDto() :
                new DhlMainResponseDto
                {
                    MainFaultCode = faultInfo.Element("faultcode")?.Value ?? string.Empty,
                    MainFaultText = faultInfo.Element("faultstring")?.Value ?? string.Empty,
                    Responses = ImmutableList.Create
                    (
                        new DhlResponseDto
                        {
                            ErrorCode = faultInfo.Element("detail")
                                ?.Element("ResponseStatus")
                                ?.Attribute("statusCode")?.Value ?? string.Empty,
                            ErrorText = faultInfo.Element("detail")
                                ?.Element("ResponseStatus")
                                ?.Attribute("statusText")?.Value ?? string.Empty,
                            StatusMessages = ImmutableHashSet.Create
                            (
                                StringComparer.InvariantCultureIgnoreCase,
                                faultInfo.Element("detail")
                                    ?.Element("ResponseStatus")
                                    ?.Attribute("statusMessage")
                                    ?.Value
                            )
                        }
                    )
                };
        }

        /// <summary>
        /// Standard response deserialization mechanism - works for both valid and invalid informations.
        /// Deserializes multiple infos if they are available.
        /// </summary>
        /// <param name="document"></param>
        private static DhlMainResponseDto DeserializeInfo(XDocument document)
        {
            var info = document?.Root?.Element("Body")?.Element("ValidateShipmentResponse");
            if (info is null) return new DhlMainResponseDto();

            return new DhlMainResponseDto
            {
                MainFaultCode = info.Element("Status")?.Element("statusCode")?.Value ?? string.Empty,
                MainFaultText = info.Element("Status")?.Element("statusText")?.Value ?? string.Empty,
                Responses = info
                    .Elements("ValidationState")
                    .Select(e =>
                    {
                        return new DhlResponseDto()
                        {
                            Key = e.Element("sequenceNumber")?.Value ?? string.Empty,
                            ErrorCode = e.Element("Status")
                                ?.Element("statusCode")?.Value ?? string.Empty,
                            ErrorText = e.Element("Status")
                                ?.Element("statusText")?.Value ?? string.Empty,
                            StatusMessages = e.Element("Status")
                                             ?.Elements("statusMessage")
                                             .Select(el => el.Value)
                                             .ToImmutableHashSet(StringComparer.InvariantCultureIgnoreCase) ?? ImmutableHashSet<string>.Empty
                        };
                    }).ToImmutableList()
            };
        }
    }
}