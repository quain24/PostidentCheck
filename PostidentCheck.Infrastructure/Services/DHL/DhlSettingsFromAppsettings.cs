namespace Postident.Infrastructure.Services.DHL
{
    public class DhlSettingsFromAppsettings : CarrierSettingsFromAppsettings
    {
        /// <summary>
        /// Additional guid for second pair of authorization values passed in every request inside request string
        /// </summary>
        public string XmlSecret { get; set; }
    }
}