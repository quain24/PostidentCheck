using Moq.AutoMock;
using Postident.Application.Common.Models;
using Postident.Infrastructure.Services.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class SingleShipmentBuilderTests
    {
        private ITestOutputHelper Output { get; }

        public SingleShipmentBuilderTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_single_shipment_element_when_correctly_built()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();
            var test = new SingleShipmentBuilder("123", new Address
            {
                City = "a",
                CountryCode = "de",
                Name = "a",
                PostIdent = "123456",
                Street = "a",
                StreetNumber = "1",
                ZipCode = "12345"
            }, TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            test
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment();

            Output.WriteLine(actual.First().ToString());
            Assert.True(actual.Count == 1);
            Assert.Equal(RawXmlDataFixtures.TestSingleShipmentA(), actual.First().ToString());
        }

        [Fact]
        public void Creates_single_international_shipment_element_when_correctly_built()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();
            var test = new SingleShipmentBuilder("123", new Address
            {
                City = "a",
                CountryCode = "at",
                Name = "a",
                PostIdent = "123456",
                Street = "a",
                StreetNumber = "1",
                ZipCode = "12345"
            }, TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            test
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment();

            Output.WriteLine(actual.First().ToString());
            Assert.True(actual.Count == 1);
            Assert.Equal(RawXmlDataFixtures.TestSingleInternationalShipment(), actual.First().ToString());
        }

        [Fact]
        public void Creates_multiple_shipment_elements_when_correctly_built()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();
            var test = new SingleShipmentBuilder("123", new Address
            {
                City = "a",
                CountryCode = "de",
                Name = "a",
                PostIdent = "123456",
                Street = "a",
                StreetNumber = "1",
                ZipCode = "12345"
            }, TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            test
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .BuildShipment();

            test
                .SetUpId("456")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .SetUpReceiverData(new Address
                {
                    City = "b",
                    CountryCode = "de",
                    Name = "b",
                    PostIdent = "123456",
                    Street = "b",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .BuildShipment();

            Output.WriteLine(actual.First().ToString());
            Output.WriteLine(actual.ElementAt(1).ToString());

            Assert.True(actual.Count == 2);
            Assert.Equal(RawXmlDataFixtures.TestSingleShipmentA(), actual.First().ToString());
            Assert.Equal(RawXmlDataFixtures.TestSingleShipmentB(), actual.ElementAt(1).ToString());
        }

        [Fact]
        public void Throws_arg_null_exc_if_address_is_null()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new SingleShipmentBuilder("123", null, TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);
            });
        }

        [Fact]
        public void Throws_arg_null_exc_if_id_not_provided()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new SingleShipmentBuilder(string.Empty, new Address
                {
                    City = "a",
                    CountryCode = "dd",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                }, TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, new List<XElement>());
            });
        }

        [Fact]
        public void Throws_arg_out_of_range_exc_with_correct_param_name_if_given_empty_string_on_SetExportDocument()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();

            var exc = Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var sb = new SingleShipmentBuilder("123", new Address
                {
                    City = "a",
                    CountryCode = "dd",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                }, TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, new List<XElement>());

                sb.SetUpExportDocument("A valid type", "valid export description", "", "valid country code", 10, 10, 10);
            });

            Output.WriteLine(exc.ToString());
            Assert.Equal("description", exc.ParamName);
        }
    }
}