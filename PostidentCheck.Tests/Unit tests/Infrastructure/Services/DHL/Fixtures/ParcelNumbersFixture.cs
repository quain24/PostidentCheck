using System.Collections.Generic;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class ParcelNumbersFixture
    {
        public static readonly IDictionary<int, string> ProperParcelNumbers = new Dictionary<int, string>()
        {
            {0, "00340434161094022102"},
            {1, "00340434161094022115"},
            {2, "00340434161094027318"},
            {3, "00340434161094032954"}
        };
    }
}