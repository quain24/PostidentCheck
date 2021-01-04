using KeePass.Models;
using Postident.Infrastructure.Interfaces.DHL;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services.DHL
{
    public class DhlOfflineSettings : IDhlSettings
    {
        public DhlOfflineSettings(DhlSettingsFromAppsettings settings, string login, string password, string xmlLogin, string xmlPassword)
        {
            MaxQueriesPerSecond = settings.MaxQueriesPerSecond;
            MaxValidationsInQuery = settings.MaxValidationsInQuery;
            BaseAddress = settings.BaseAddress;
            Settings = settings;
            Login = login;
            Password = password;
            XmlLogin = xmlLogin;
            XmlPassword = xmlPassword;
        }

        public int MaxValidationsInQuery { get; set; }
        public string BaseAddress { get; set; }
        public int MaxQueriesPerSecond { get; set; }
        public DhlSettingsFromAppsettings Settings { get; }
        public string Login { get; }
        public string Password { get; }
        public string XmlLogin { get; }
        public string XmlPassword { get; }

        public Task<Secret> Secret(CancellationToken ct)
        {
            return Task.FromResult(new Secret() { Username = Login, Password = Password, Id = "secret" });
        }

        public Task<Secret> XmlSecret(CancellationToken ct)
        {
            return Task.FromResult(new Secret() { Username = XmlLogin, Password = XmlPassword, Id = "secret" });
        }

        public class AppSet
        {
            public string Login { get; set; }
            public string XmlLogin { get; set; }
            public string Password { get; set; }
            public string XmlPassword { get; set; }
        }
    }
}