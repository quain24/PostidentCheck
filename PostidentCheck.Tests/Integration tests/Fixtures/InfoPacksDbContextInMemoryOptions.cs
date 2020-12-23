using System;
using Microsoft.EntityFrameworkCore;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public static class InfoPacksDbContextInMemoryOptions
    {
        public static DbContextOptions<InfoPacksDbContextInMemory> GetOptions()
        {
            return new DbContextOptionsBuilder<InfoPacksDbContextInMemory>()
                // DateTime solves issues with tests trying to use same database causing exceptions / intermediate failures
                .UseInMemoryDatabase("InfoPack" + +DateTime.Now.ToFileTimeUtc())
                .Options;
        }
    }
}