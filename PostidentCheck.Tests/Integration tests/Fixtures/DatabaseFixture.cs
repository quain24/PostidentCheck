using Postident.Core.Entities;
using Postident.Core.Enums;
using PostidentCheck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class DatabaseFixture
    {
        private static List<DataPackReadModel> ReadModelSeedData;
        private static List<InfoPackWriteModel> WriteModelSeedData;

        private static Func<List<DataPackReadModel>> GetDataPacks = PopulateDataPacksSeedData;
        private static Func<List<InfoPackWriteModel>> GetInfoPacks = PopulateInfoPacksSeedData;

        private static List<DataPackReadModel> PopulateDataPacksSeedData()
        {
            if (ReadModelSeedData != null)
                return ReadModelSeedData;

            var fileLocalization = Directory.GetCurrentDirectory()
                .Replace(Assembly.GetAssembly(typeof(Program)).GetName().ToString(),
                    Assembly.GetAssembly(typeof(DatabaseFixture)).GetName().ToString());

            var parcelSeedDataJsonModel =
                JsonSerializer.Deserialize<List<DataPackJsonCompatibleModel>>(File.ReadAllText(fileLocalization +
                    "\\Integration tests\\Fixtures\\InMemoryReadDb.json"));

            ReadModelSeedData = parcelSeedDataJsonModel.Select(p => new DataPackReadModel()
            {
                Street = p.Street,
                PostIdent = p.PostIdent,
                City = p.City,
                CountryCode = p.CountryCode,
                Carrier = p.Carrier,
                DataPackChecked = p.DataPackChecked,
                Id = p.Id,
                ZipCode = p.ZipCode
            }).ToList();

            GetDataPacks = CloneDataPackDataSeed;
            return ReadModelSeedData;
        }

        private static List<InfoPackWriteModel> PopulateInfoPacksSeedData()
        {
            if (WriteModelSeedData != null)
                return WriteModelSeedData;

            var fileLocalization = Directory.GetCurrentDirectory()
                .Replace(Assembly.GetAssembly(typeof(Program)).GetName().ToString(),
                    Assembly.GetAssembly(typeof(DatabaseFixture)).GetName().ToString());

            var parcelSeedDataJsonModel =
                JsonSerializer.Deserialize<List<InfoPackJsonCompatibleModel>>(File.ReadAllText(fileLocalization +
                    "\\Integration tests\\Fixtures\\InMemoryWriteDb.json"));

            WriteModelSeedData = parcelSeedDataJsonModel.Select(p => new InfoPackWriteModel()
            {
                CheckStatus = p.CheckStatus,
                Id = p.Id,
                Message = p.Message
            }).ToList();

            GetInfoPacks = CloneInfoPackDataSeed;
            return WriteModelSeedData;
        }

        private static List<DataPackReadModel> CloneDataPackDataSeed()
        {
            return ReadModelSeedData.Select(p =>
            {
                return new DataPackReadModel()
                {
                    Street = p.Street,
                    PostIdent = p.PostIdent,
                    City = p.City,
                    CountryCode = p.CountryCode,
                    Carrier = p.Carrier,
                    DataPackChecked = p.DataPackChecked,
                    Id = p.Id,
                    ZipCode = p.ZipCode
                };
            }).ToList();
        }

        private static List<InfoPackWriteModel> CloneInfoPackDataSeed()
        {
            return WriteModelSeedData.Select(p =>
            {
                return new InfoPackWriteModel()
                {
                    CheckStatus = p.CheckStatus,
                    Id = p.Id,
                    Message = p.Message
                };
            }).ToList();
        }

        public static DataPacksDbContextInMemory CreateDataPackPopulatedDatabaseNoStateDbContext()
        {
            var context = new DataPacksDbContextInMemory(DataPacksDbContextInMemoryOptions.GetOptions());
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.DataPacks.AddRange(GetDataPacks());
            context.SaveChanges();
            return context;
        }

        public static InfoPacksDbContextInMemory CreateInfoPackPopulatedDatabaseNoStateDbContext()
        {
            var context = new InfoPacksDbContextInMemory(InfoPacksDbContextInMemoryOptions.GetOptions());
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.InfoPacks.AddRange(GetDataPacks());
            context.SaveChanges();
            return context;
        }

        private class DataPackJsonCompatibleModel
        {
            public string Id { get; set; }
            public Carrier Carrier { get; set; }
            public string PostIdent { get; set; }
            public string Street { get; set; }
            public string ZipCode { get; set; }
            public string City { get; set; }
            public string CountryCode { get; set; }

            /// <summary>
            /// -1 - not checked, 0 - checked, contains errors, 1 - valid
            /// </summary>
            public int DataPackChecked { get; set; }
        }

        private class InfoPackJsonCompatibleModel
        {
            public string Id { get; set; }
            public InfoPackCheckStatus CheckStatus { get; set; }
            public string Message { get; set; }
        }
    }
}