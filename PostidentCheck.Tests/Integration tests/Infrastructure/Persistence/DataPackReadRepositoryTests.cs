using Postident.Core.Enums;
using Postident.Infrastructure.Persistence;
using Postident.Tests.Integration_tests.Fixtures;
using System;
using System.Threading;
using System.Threading.Tasks;
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
        [InlineData("1000757941")]
        [InlineData("1000757942")]
        [InlineData("1000757943")]
        public async Task Will_return_correct_DataPack_when_given_valid_id(string id)
        {
            Repository = new DataPackReadRepository(_fixture);

            var actual = await Repository.GetDataPack(id, CancellationToken.None);

            Assert.True(actual.Count == 1);
            Assert.All(actual, parcel => Assert.True(parcel.Id == id));
        }

        [Theory]
        [InlineData("1000758322", "1000758018", "1000758017")]
        [InlineData("1000758016", "1000758014", "1000758013")]
        [InlineData("1000758010", "1000758008", "1000758007")]
        public async Task Will_return_correct_DataPacks_when_given_valid_multiple_ids(string id, string id2, string id3)
        {
            Repository = new DataPackReadRepository(_fixture);

            var actual = await Repository.GetDataPacks(new[] { id, id2, id3 }, CancellationToken.None);

            Assert.True(actual.Count == 3);
            Assert.Contains(actual, d => d.Id == id);
            Assert.Contains(actual, d => d.Id == id2);
            Assert.Contains(actual, d => d.Id == id3);
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

        [Fact]
        public async Task Will_throw_ArgumentOutOfRange_exception_when_given_empty_id()
        {
            Repository = new DataPackReadRepository(_fixture);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Repository.GetDataPack("", CancellationToken.None));
        }
    }
}