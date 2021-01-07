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
        public void Builds_xml_from_proper_data_domestic()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            var result = builder.SetUpAuthorization("usr", "pass")
                .AddNewShipment("123", new Address
                {
                    City = "a",
                    CountryCode = "de",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment()
                .Build();

            Output.WriteLine(result);
            Assert.Equal(RawXmlDataFixtures.TestValidationXmlDomesticRequest(), result);
        }

        [Fact]
        public void Builds_xml_from_proper_data_international_eu()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            var result = builder.SetUpAuthorization("usr", "pass")
                .AddNewShipment("123", new Address
                {
                    City = "a",
                    CountryCode = "AT",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment()
                .Build();

            Output.WriteLine(result);
            Assert.Equal(RawXmlDataFixtures.TestValidationXmlForeignInEuRequest(), result);
        }

        [Fact]
        public void Builds_xml_from_proper_data_international_outside_eu()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            var result = builder.SetUpAuthorization("usr", "pass")
                .AddNewShipment("123", new Address
                {
                    City = "a",
                    CountryCode = "CN",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment()
                .Build();

            Output.WriteLine(result);
            Assert.Equal(RawXmlDataFixtures.TestValidationXmlForeignOutsideEu(), result);
        }

        [Fact]
        public void Throws_missing_field_exc_if_no_shipments_were_added()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            Assert.Throws<MissingFieldException>(() => builder.SetUpAuthorization("usr", "pass").Build());
        }

        [Fact]
        public void Throws_missing_field_exc_if_no_authorization_was_added()
        {
            var builder = new ValidationRequestXmlBuilder(TestShipmentDefaultValuesFixture.Defaults());

            Assert.Throws<MissingFieldException>(() => builder
                .AddNewShipment("123", new Address
                {
                    City = "a",
                    CountryCode = "dd",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment()
                .Build());
        }
    }
}