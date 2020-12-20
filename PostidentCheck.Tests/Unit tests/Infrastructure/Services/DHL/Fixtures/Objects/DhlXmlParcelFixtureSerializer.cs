using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Postident.Infrastructure.Common;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects
{
    internal class DhlXmlParcelFixtureSerializer
    {
        public DhlResponseFixtureDto Deserialize(Stream message)
        {
            var document = XDocument.Load(message);
            var serializer = new XmlSerializer(typeof(DhlResponseFixtureDto));

            using var reader = document.CreateReader();
            return serializer.Deserialize(reader) as DhlResponseFixtureDto;
        }

        internal string Serialize(DhlResponseFixtureDto dto)
        {
            using var sw = new Utf8StringWriter();
            using var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = false });

            //xw.WriteStartDocument(false); // "standalone = no" parameter in xml
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var xmlSerializer = new XmlSerializer(typeof(DhlResponseFixtureDto));
            xmlSerializer.Serialize(xw, dto, namespaces);
            return sw.ToString();
        }
    }
}