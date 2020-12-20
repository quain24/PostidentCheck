using System;
using System.Threading;
using System.Threading.Tasks;
using Postident.Core.Enums;
using Postident.Infrastructure.Persistence;
using Postident.Tests.Integration_tests.Fixtures;
using Xunit;

namespace Postident.Tests.Integration_tests.Infrastructure.Persistence
{
    public class ParcelRepositoryTests
    {
        private readonly DbContextInMemory _fixture;
        private DataPackRepository Repository { get; set; }

        public ParcelRepositoryTests()
        {
            _fixture = DatabaseFixture.CreatePopulatedDatabaseNoStateDbContext();
        }

        [Theory]
        [InlineData(Carrier.DHL)]
        [InlineData(Carrier.DHL_Sperr)]
        [InlineData(Carrier.DPD)]
        public async Task Will_return_correct_parcels_when_given_valid_courier_name(Carrier carrier)
        {
            Repository = new DataPackRepository(_fixture);

            var actual = await Repository.GetParcelsSentBy(carrier, CancellationToken.None);

            Assert.True(actual.Count > 0);
            Assert.All(actual, parcel => Assert.True(parcel.CarrierId == ((int)carrier).ToString()));
        }

        [Theory]
        [InlineData(Carrier.DHL, Carrier.DHL_Sperr)]
        [InlineData(Carrier.DHL_Sperr, Carrier.DPD)]
        [InlineData(Carrier.DHL, Carrier.DPD)]
        public async Task Will_return_correct_parcels_when_given_multiple_valid_courier_names(Carrier carrier, Carrier anotherCarrier)
        {
            Repository = new DataPackRepository(_fixture);

            var actual = await Repository.GetParcelsSentBy(new[] { carrier, anotherCarrier }, CancellationToken.None);

            Assert.Contains(actual, parcel => parcel.CarrierId == ((int)carrier).ToString());
            Assert.Contains(actual, parcel => parcel.CarrierId == ((int)anotherCarrier).ToString());
            Assert.All(actual, parcel => Assert.True(parcel.CarrierId == ((int)carrier).ToString() || parcel.CarrierId == ((int)anotherCarrier).ToString()));
        }

        [Fact]
        public async Task Will_throw_ArgumentOutOfRange_exception_when_given_courier_out_of_enum()
        {
            Repository = new DataPackRepository(_fixture);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Repository.GetParcelsSentBy((Carrier)15100900, CancellationToken.None));
        }

        [Fact]
        public async Task Should_save_changes_to_database()
        {
            Repository = new DataPackRepository(_fixture);

            var parcels = await Repository.GetParcelsSentBy(Carrier.DHL, CancellationToken.None);
            parcels.ForEach(p => p.ParcelStatus = "Updated status");
            await Repository.UpdateDataPacksTableAsync();

            Assert.All(await Repository.GetParcelsSentBy(Carrier.DHL, CancellationToken.None), p => Assert.True(p.ParcelStatus == "Updated status"));
        }
    }
}