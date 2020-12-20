using KeePass;
using KeePass.Models;
using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Interfaces.DHL;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services.DHL
{
    /// <summary>
    /// Configuration class for DHL service, contains configuration from appsettings and can access KeePass service for secrets
    /// </summary>
    internal class DhlSettingsFromPasswordServer : IDhlSettings
    {
        private readonly IKeePassService _keePassService;

        /// <summary>
        /// Create configuration object for DHL service
        /// </summary>
        /// <param name="keePassService">A <see cref="Secret"/> provider</param>
        /// <param name="settings">Settings taken from json file</param>
        public DhlSettingsFromPasswordServer(IKeePassService keePassService, DhlSettingsFromAppsettings settings)
        {
            _keePassService = keePassService ?? throw new ArgumentNullException(nameof(keePassService), "Missing password service instance.");

            CheckSettings(settings);
            AssignSettings(settings);
        }

        private void CheckSettings(DhlSettingsFromAppsettings settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings), "Missing proper instance of settings - could not setup necessary settings.");

            var infos = settings.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            infos.ForEach(pi =>
            {
                switch (pi.GetValue(settings))
                {
                    case null:
                        throw new ArgumentNullException(nameof(settings), $"\"{pi.Name}\" passed in {nameof(settings)} is null - possibly a missing string / group in appsettings.json");
                    case int number when number <= 0:
                        throw new ArgumentOutOfRangeException(nameof(settings), $"\"{pi.Name}\" from {nameof(settings)} is <= 0 - either it has bad value or it (or whole group) is missing from appsettings.json");
                }
            });
        }

        private void AssignSettings(DhlSettingsFromAppsettings settings)
        {
            BaseAddress = settings.BaseAddress;
            MaxQueriesPerSecond = settings.MaxQueriesPerSecond;
            XmlSecretGuid = settings.XmlSecret;
            SecretGuid = settings.Secret;
        }

        /// <summary>
        /// <inheritdoc cref="IDhlSettings.XmlSecret()"/>
        /// </summary>
        /// <exception cref="System.OperationCanceledException()"></exception>
        public Task<Secret> XmlSecret(CancellationToken ct) => _keePassService.AskForSecret(XmlSecretGuid, ct);

        /// <summary>
        /// <inheritdoc cref="ICarrierServiceSettings.Secret()"/>
        /// </summary>
        /// <exception cref="System.OperationCanceledException()"></exception>
        public Task<Secret> Secret(CancellationToken ct) => _keePassService.AskForSecret(SecretGuid, ct);

        /// <summary>
        /// <inheritdoc cref="ICarrierServiceSettings.BaseAddress"/>
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// <inheritdoc cref="ICarrierServiceSettings.MaxQueriesPerSecond"/>
        /// </summary>
        public int MaxQueriesPerSecond { get; set; }

        private string XmlSecretGuid { get; set; }
        private string SecretGuid { get; set; }
    }
}