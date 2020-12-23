using System;
using System.Threading;
using System.Threading.Tasks;
using Postident.Core.Enums;
using Postident.Infrastructure.Persistence;
using Postident.Tests.Integration_tests.Fixtures;
using Xunit;

namespace Postident.Tests.Integration_tests.Infrastructure.Persistence
{
    public class DataPackReadRepositoryTests
    {
        private readonly DataPacksDbContextInMemory _fixture;
        private DataPackReadRepository Repository { get; set; }

        public DataPackReadRepositoryTests()
        {
            _fixture = DatabaseFixture.CreateDataPackPopulatedDatabaseNoStateDbContext();
        }

        [Theory]
        [InlineData(Carrier.DHL)]
        [InlineData(Carrier.DHL_Sperr)]
        [InlineData(Carrier.DPD)]
        public async Task Will_return_correct_DataPacks_when_given_valid_courier_name(Carrier carrier)
        {
            Repository = new DataPackReadRepository(_fixture);

            var actual = await Repository.GetDataPacks(carrier, CancellationToken.None);

            Assert.True(actual.Count > 0);
            Assert.All(actual, parcel => Assert.True(parcel.Carrier == carrier));
        }

        [Theory]
        [InlineData(Carrier.DHL, Carrier.DHL_Sperr)]
        [InlineData(Carrier.DHL_Sperr, Carrier.DPD)]
        [InlineData(Carrier.DHL, Carrier.DPD)]
        public async Task Will_return_correct_DataPacks_when_given_multiple_valid_courier_names(Carrier carrier, Carrier anotherCarrier)
        {
            Repository = new DataPackReadRepository(_fixture);

            var actual = await Repository.GetDataPacks(new[] { carrier, anotherCarrier }, CancellationToken.None);

            Assert.Contains(actual, parcel => parcel.Carrier == carrier);
            Assert.Contains(actual, parcel => parcel.Carrier == anotherCarrier);
            Assert.All(actual, parcel => Assert.True(parcel.Carrier == carrier || parcel.Carrier == anotherCarrier));
        }

        [Fact]
        public async Task Will_throw_ArgumentOutOfRange_exception_when_given_carrier_out_of_enum()
        {
            Repository = new DataPackReadRepository(_fixture);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Repository.GetDataPacks((Carrier)15100900, CancellationToken.None));
        }
    }
}