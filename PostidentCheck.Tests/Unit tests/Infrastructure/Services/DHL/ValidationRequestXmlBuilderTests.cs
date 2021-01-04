using KeePass.Models;
using Postident.Application.Common.Models;
using Postident.Infrastructure.Services.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class ValidationRequestXmlBuilderTests
    {
        private ITestOutputHelper Output { get; }

        public ValidationRequestXmlBuilderTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Builds_xml_from_proper_data()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            var result = builder.SetUpAuthorization(new Secret() { Password = "pass", Username = "usr", Id = "1" })
                .AddNewShipment()
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .SetUpReceiverData(new Address
                {
                    City = "a",
                    CountryCode = "dd",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .BuildShipment()
                .Build();

            Output.WriteLine(result);
            Assert.Equal(RawXmlDataFixtures.TestValidationXmlRequest(), result);
        }

        [Fact]
        public void Throws_missing_field_exc_if_no_shipments_were_added()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            Assert.Throws<MissingFieldException>(() => builder.SetUpAuthorization(new Secret { Password = "pass", Username = "usr", Id = "1" }).Build());
        }

        [Fact]
        public void Throws_missing_field_exc_if_no_authorization_was_added()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            Assert.Throws<MissingFieldException>(() => builder
                .AddNewShipment()
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .SetUpReceiverData(new Address
                {
                    City = "a",
                    CountryCode = "dd",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .BuildShipment()
                .Build());
        }
    }
}