using System;
using KeePass;
using Moq.AutoMock;
using Postident.Infrastructure.Services.DHL;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class DhlSettingsFromPasswordServerTests
    {
        private ITestOutputHelper Output { get; }

        public DhlSettingsFromPasswordServerTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Will_throw_ArgNullExc_if_no_KeePass_service_passed_in_constructor()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DhlSettingsFromPasswordServer(null, new DhlSettingsFromAppsettings()));
        }

        [Fact]
        public void Will_throw_ArgNullExc_if_no_settings_passed_in_constructor()
        {
            var test = new AutoMocker().GetMock<IKeePassService>();
            Assert.Throws<ArgumentNullException>(() =>
                new DhlSettingsFromPasswordServer(test.Object, null));
        }

        [Fact]
        public void Will_throw_ArgNullExc_if_one_of_passed_settings_properties_is_null()
        {
            var test = new AutoMocker().GetMock<IKeePassService>();

            var e = Assert.Throws<ArgumentNullException>(() =>
                new DhlSettingsFromPasswordServer(test.Object, new DhlSettingsFromAppsettings()
                {
                    BaseAddress = null,
                    LanguageCode = "en",
                    MaxParcelNumbersInQuery = 10,
                    MaxQueriesPerSecond = 10,
                    Secret = "aa",
                    XmlSecret = "bb"
                }));
            Output.WriteLine(e.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Will_throw_ArgOutOfRangeExc_if_one_of_passed_numerical_settings_properties_is_le_0(int val)
        {
            var test = new AutoMocker().GetMock<IKeePassService>();

            var e = Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DhlSettingsFromPasswordServer(test.Object, new DhlSettingsFromAppsettings()
                {
                    BaseAddress = "base",
                    LanguageCode = "en",
                    MaxParcelNumbersInQuery = 10,
                    MaxQueriesPerSecond = val,
                    Secret = "aa",
                    XmlSecret = "bb"
                }));
            Output.WriteLine(e.Message);
        }
    }
}