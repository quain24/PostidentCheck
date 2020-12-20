using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Postident.Core.Entities;
using PostidentCheck;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class DatabaseFixture
    {
        private static List<DataPack> ParcelSeedData;

        private static Func<List<DataPack>> GetParcels = PopulateParcelSeedData;

        private static List<DataPack> PopulateParcelSeedData()
        {
            if (ParcelSeedData != null)
                return ParcelSeedData;

            var fileLocalization = Directory.GetCurrentDirectory()
                .Replace(Assembly.GetAssembly(typeof(Program)).GetName().ToString(),
                    Assembly.GetAssembly(typeof(DatabaseFixture)).GetName().ToString());

            var parcelSeedDataJsonModel =
                JsonSerializer.Deserialize<List<ParcelJsonCompatibleModel>>(File.ReadAllText(fileLocalization +
                    "\\Integration tests\\Fixtures\\InMemoryDbContent.json"));

            ParcelSeedData = parcelSeedDataJsonModel.Select(p => new DataPack()
            {
                CarrierId = p.carrier,
                ParcelId = p.parcelId,
                ParcelStatus = p.parcelStatus,
                TrackingNumber = p.tracking
            }).ToList();

            GetParcels = CloneParcelDataSeed;
            return ParcelSeedData;
        }

        private static List<DataPack> CloneParcelDataSeed()
        {
            return ParcelSeedData.Select(p =>
            {
                return new DataPack()
                {
                    CarrierId = p.CarrierId,
                    TrackingNumber = p.TrackingNumber,
                    ParcelStatus = p.ParcelStatus,
                    ParcelId = p.ParcelId
                };
            }).ToList();
        }

        public static DbContextInMemory CreatePopulatedDatabaseNoStateDbContext()
        {
            var context = new DbContextInMemory(DbContextInMemoryOptions.GetOptions());
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Parcels.AddRange(GetParcels());
            context.SaveChanges();
            return context;
        }

        private class ParcelJsonCompatibleModel
        {
            public string carrier { get; set; }
            public int parcelId { get; set; }
            public string tracking { get; set; }
            public string parcelStatus { get; set; }
        }
    }
}