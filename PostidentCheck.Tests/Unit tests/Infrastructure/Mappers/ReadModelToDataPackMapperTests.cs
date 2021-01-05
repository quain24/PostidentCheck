using Microsoft.Extensions.Logging;
using Postident.Core.Entities;
using Postident.Core.Enums;
using Postident.Infrastructure.Mappers;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Mappers
{
    public class ReadModelToDataPackMapperTests
    {
        public ReadModelToDataPackMapperTests(ITestOutputHelper output)
        {
            Output = output;
            Logger = output.BuildLoggerFor<ReadModelToDataPackMapper>();
            Mapper = new ReadModelToDataPackMapper(Logger);
        }

        private ITestOutputHelper Output { get; }
        private ILogger<ReadModelToDataPackMapper> Logger { get; }
        private ReadModelToDataPackMapper Mapper { get; }

        public static IEnumerable<object[]> ValidDataPackReadModels =>
            new List<object[]>
            {
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Lippstadt",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "1", PostIdent = "",
                        Street = "Schulstraße 89",
                        ZipCode = "123456"
                    }, "Schulstraße", "89","Lippstadt","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"1","","123456"},
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Lippstadt",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "2", PostIdent = "",
                        Street = "Westernkötter Straße 8",
                        ZipCode = "123456"
                    }, "Westernkötter Straße", "8","Lippstadt","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"2","","123456"},
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Lippstadt",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "3", PostIdent = "",
                        Street = "Hammeltrift 32 A",
                        ZipCode = "123456"
                    }, "Hammeltrift", "32 A","Lippstadt","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"3","","123456"},
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Lippstadt",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "4", PostIdent = "",
                        Street = "Friedrich-von-Gärtner-Ring 7",
                        ZipCode = "123456"
                    }, "Friedrich-von-Gärtner-Ring", "7","Lippstadt","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"4","","123456"},
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Augsburg",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "5", PostIdent = "",
                        Street = "Alexander-Pachmann-Str. 27",
                        ZipCode = "123456"
                    }, "Alexander-Pachmann-Str.", "27","Augsburg","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"5","","123456"},
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Berlin",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "6", PostIdent = "857907118",
                        Street = "Packstation 146",
                        ZipCode = "123456"
                    }, "Packstation", "146","Berlin","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"6","857907118","123456"}
            };

        [Theory]
        [MemberData(nameof(ValidDataPackReadModels))]
        public void Will_convert_valid_model_to_valid_data_pack_object(DataPackReadModel source,
            string streetName, string streetNumber, string cityName, string countryCode,
            Carrier carrier, InfoPackCheckStatus status, string id, string postident, string zipCode)
        {
            var result = Mapper.Map(source);
            var actual = result.Address;
            Output.WriteLine(result.Address.Street + " | " + result.Address.StreetNumber);

            Assert.Equal(streetName, actual.Street);
            Assert.Equal(streetNumber, actual.StreetNumber);
            Assert.Equal(cityName, actual.City);
            Assert.Equal(countryCode, actual.CountryCode);
            Assert.Equal(carrier, result.Carrier);
            Assert.Equal(status, result.DataPackChecked);
            Assert.Equal(id, result.Id);
            Assert.Equal(zipCode, actual.ZipCode);
            Assert.Equal(postident, actual.PostIdent);
        }

        [Fact]
        public void Will_convert_read_model_without_street_number_into_valid_data_pack()
        {
            var source = new DataPackReadModel
            {
                Carrier = Carrier.DHL,
                City = "Berlin",
                CountryCode = "DE",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "6",
                PostIdent = "857907118",
                Street = "Alberta",
                ZipCode = "123456"
            };

            var result = Mapper.Map(source);

            Assert.Equal(source.Street, result.Address.Street);
            Assert.Empty(result.Address.StreetNumber);
            Assert.Equal(source.City, result.Address.City);
            Assert.Equal(source.CountryCode, result.Address.CountryCode);
            Assert.Equal(source.Carrier, result.Carrier);
            Assert.Equal(source.DataPackChecked, result.DataPackChecked);
            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.ZipCode, result.Address.ZipCode);
            Assert.Equal(source.PostIdent, result.Address.PostIdent);
        }

        public static IEnumerable<object[]> DirtyDataPackReadModels =>
           new List<object[]>
           {
               new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = " Lippstadt",
                        CountryCode = "DE  ",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "3", PostIdent = "",
                        Street = "Hammeltrift    32 A1",
                        ZipCode = "123456"
                    }, "Hammeltrift    32 A", "1","Lippstadt","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"3","","123456"},
                new object[] {
                    new DataPackReadModel
                    {
                        Carrier = Carrier.DHL,
                        City = "Lippstadt     ",
                        CountryCode = "DE",
                        DataPackChecked = InfoPackCheckStatus.Unchecked,
                        Id = "  4", PostIdent = "1",
                        Street = "Friedrich - von - Gärtner - Ring     7  ",
                        ZipCode = "123456"
                    }, "Friedrich - von - Gärtner - Ring", "7","Lippstadt","DE",Carrier.DHL,InfoPackCheckStatus.Unchecked,"4","1","123456"}
           };

        [Theory]
        [MemberData(nameof(DirtyDataPackReadModels))]
        public void Will_convert_dirty_but_readable_model_to_valid_data_pack_object(DataPackReadModel source,
            string streetName, string streetNumber, string cityName, string countryCode,
            Carrier carrier, InfoPackCheckStatus status, string id, string postident, string zipCode)
        {
            var result = Mapper.Map(source);
            var actual = result.Address;
            Output.WriteLine(source.Street + " || " + result.Address.Street + " | " + result.Address.StreetNumber);

            Assert.Equal(streetName, actual.Street);
            Assert.Equal(streetNumber, actual.StreetNumber);
            Assert.Equal(cityName, actual.City);
            Assert.Equal(countryCode, actual.CountryCode);
            Assert.Equal(carrier, result.Carrier);
            Assert.Equal(status, result.DataPackChecked);
            Assert.Equal(id, result.Id);
            Assert.Equal(zipCode, actual.ZipCode);
            Assert.Equal(postident, actual.PostIdent);
        }
    }
}