using Postident.Infrastructure.Common;

namespace Postident.Infrastructure.Services.DHL
{
    public class DhlSettingsFromAppsettings : CarrierSettingsFromAppsettings
    {
        /// <summary>
        /// Additional guid for second pair of authorization values passed in every request inside request string
        /// </summary>
        public string XmlSecret { get; set; }

        /// <summary>
        /// How many validations can be sent to api in a single request - maximum 20
        /// </summary>
        public int MaxValidationsInQuery { get; set; }
    }
}