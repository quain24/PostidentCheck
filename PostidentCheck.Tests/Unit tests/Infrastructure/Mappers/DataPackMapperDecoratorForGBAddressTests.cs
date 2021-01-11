using Moq;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using Postident.Core.Enums;
using Postident.Infrastructure.Mappers;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Mappers
{
    public class DataPackMapperDecoratorForGBAddressTests
    {
        public DataPackMapperDecoratorForGBAddressTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private IReadModelToDataPackMapper Mapper { get; set; }
        private ITestOutputHelper Output { get; }

        [Fact]
        public void Will_set_street_number_to_one_if_address_has_none_and_is_gb()
        {
            var mock = new Mock<IReadModelToDataPackMapper>();
            mock.Setup(m => m.Map(It.IsAny<DataPackReadModel>())).Returns(new DataPack()
            {
                Address = new Address()
                {
                    City = "London",
                    CountryCode = "GB",
                    Name = "Private",
                    PostIdent = "",
                    Street = "Jemy str.",
                    StreetNumber = "",
                    ZipCode = "AC 25482"
                },
                Carrier = Carrier.DHL,
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "1234"
            });

            Mapper = mock.Object;
            var test = new DataPackMapperDecoratorForGBAddresses(Mapper,
                Output.BuildLoggerFor<DataPackMapperDecoratorForGBAddresses>());

            var actual = test.Map(new DataPackReadModel());

            Assert.True(actual.Address.StreetNumber == "1");
        }

        [Fact]
        public void Will_leave_street_number_if_address_has_one_and_is_gb()
        {
            var mock = new Mock<IReadModelToDataPackMapper>();
            mock.Setup(m => m.Map(It.IsAny<DataPackReadModel>())).Returns(new DataPack()
            {
                Address = new Address()
                {
                    City = "London",
                    CountryCode = "GB",
                    Name = "Private",
                    PostIdent = "",
                    Street = "Jemy str.",
                    StreetNumber = "22",
                    ZipCode = "AC 25482"
                },
                Carrier = Carrier.DHL,
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "1234"
            });

            Mapper = mock.Object;
            var test = new DataPackMapperDecoratorForGBAddresses(Mapper,
                Output.BuildLoggerFor<DataPackMapperDecoratorForGBAddresses>());

            var actual = test.Map(new DataPackReadModel());

            Assert.True(actual.Address.StreetNumber == "22");
        }
    }
}