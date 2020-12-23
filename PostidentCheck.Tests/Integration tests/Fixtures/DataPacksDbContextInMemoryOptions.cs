using System;
using Microsoft.EntityFrameworkCore;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public static class DataPacksDbContextInMemoryOptions
    {
        public static DbContextOptions<DataPacksDbContextInMemory> GetOptions()
        {
            return new DbContextOptionsBuilder<DataPacksDbContextInMemory>()
                // DateTime solves issues with tests trying to use same database causing exceptions / intermediate failures
                .UseInMemoryDatabase("DataPack" + +DateTime.Now.ToFileTimeUtc())
                .Options;
        }
    }
}