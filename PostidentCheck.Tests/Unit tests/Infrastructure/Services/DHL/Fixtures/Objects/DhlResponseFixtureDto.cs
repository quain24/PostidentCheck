using System.Collections.Generic;
using System.Xml.Serialization;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects
{
    [XmlRoot(ElementName = "data")]
    [XmlType("data")]
    public class DhlResponseFixtureDto
    {
        public static DhlResponseFixtureDto Empty()
        {
            return new DhlResponseFixtureDto()
            {
                Parcels = new List<DhlResponseParcelFixtureDto>(0),
                Name = string.Empty,
                RequestId = string.Empty,
                ErrorText = string.Empty,
            };
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("request-id")]
        public string RequestId { get; set; }

        [XmlAttribute("code")]
        public int ErrorCode { get; set; }

        [XmlAttribute("error")]
        public string ErrorText { get; set; }

        [XmlElement("data")]
        public List<DhlResponseParcelFixtureDto> Parcels { get; set; } = new List<DhlResponseParcelFixtureDto>();
    }
}