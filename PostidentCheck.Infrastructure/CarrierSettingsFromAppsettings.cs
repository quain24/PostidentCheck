namespace Postident.Infrastructure
{
    public class CarrierSettingsFromAppsettings
    {
        /// <summary>
        /// Base address used to configure services <see cref="System.Net.Http.HttpClient"/> instance. Must end with "/" sign.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// guid for password service to find correct password and login
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Maximum number of requests sent by <see cref="System.Net.Http.HttpClient"/> in a single second.
        /// </summary>
        public int MaxQueriesPerSecond { get; set; }
    }
}