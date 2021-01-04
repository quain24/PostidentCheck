using KeePass.Models;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class DhlSettingsFixture
    {
        public static DhlSettings GetProperSettings(int maxQueriesPerSecond, int maxValidationsInQuery)
        {
            return new(new Secret { Username = "app_name_http_client", Password = "app_token_http_client" }, new Secret { Username = "ztLogin", Password = "ztPassword" })
            {
                BaseAddress = "https://www.dhltest.com/",
                MaxValidationsInQuery = maxValidationsInQuery,
                MaxQueriesPerSecond = maxQueriesPerSecond
            };
        }
    }
}