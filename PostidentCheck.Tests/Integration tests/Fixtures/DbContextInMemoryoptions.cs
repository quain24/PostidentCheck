using System;
using Microsoft.EntityFrameworkCore;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public static class DbContextInMemoryOptions
    {
        public static DbContextOptions<DbContextInMemory> GetOptions()
        {
            return new DbContextOptionsBuilder<DbContextInMemory>()
                // DateTime solves issues with tests trying to use same database causing exceptions / intermediate failures
                .UseInMemoryDatabase("Parcel" + +DateTime.Now.ToFileTimeUtc())
                .Options;
        }
    }
}