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
            var test = new SingleShipmentBuilder(TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            test
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .SetUpReceiverData(new Address
                {
                    City = "a",
                    CountryCode = "de",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
                .BuildShipment();

            Output.WriteLine(actual.First().ToString());
            Assert.True(actual.Count == 1);
            Assert.Equal(RawXmlDataFixtures.TestSingleShipmentA(), actual.First().ToString());
        }

        [Fact]
        public void Creates_multiple_shipment_elements_when_correctly_built()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();
            var test = new SingleShipmentBuilder(TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            test
                .SetUpId("123")
                .SetUpShippingDate(new DateTime(3000, 1, 1))
                .SetUpReceiverData(new Address
                {
                    City = "a",
                    CountryCode = "de",
                    Name = "a",
                    PostIdent = "123456",
                    Street = "a",
                    StreetNumber = "1",
                    ZipCode = "12345"
                })
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
        public void Throws_Missing_field_exc_if_address_not_provided()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();
            var test = new SingleShipmentBuilder(TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            Assert.Throws<MissingFieldException>(() =>
            {
                test
                    .SetUpId("123")
                    .SetUpShippingDate(new DateTime(3000, 1, 1))
                    .BuildShipment();
            });
        }

        [Fact]
        public void Throws_Missing_field_exc_if_id_not_provided()
        {
            var mocker = new AutoMocker();
            var builder = mocker.GetMock<ValidationRequestXmlBuilder>();
            var actual = new List<XElement>();
            var test = new SingleShipmentBuilder(TestShipmentDefaultValuesFixture.Defaults(), "ns", builder.Object, actual);

            Assert.Throws<MissingFieldException>(() =>
            {
                test
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
                    .BuildShipment();
            });
        }
    }
}