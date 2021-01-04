using KeePass.Models;
using Postident.Infrastructure.Interfaces.DHL;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects
{
    public class DhlSettings : IDhlSettings
    {
        private readonly Secret _secret;
        private readonly Secret _xmlSecret;

        public DhlSettings(Secret secret, Secret xmlSecret)
        {
            _secret = secret;
            _xmlSecret = xmlSecret;
        }

        public string BaseAddress { get; set; }
        public int MaxQueriesPerSecond { get; set; }

        public Task<Secret> Secret(CancellationToken ct)
        {
            return Task.FromResult(_secret);
        }

        public Task<Secret> XmlSecret(CancellationToken ct)
        {
            return Task.FromResult(_xmlSecret);
        }

        public int MaxValidationsInQuery { get; set; } = 1;
    }
}