namespace Postident.Infrastructure.Common
{
    internal class DefaultShipmentValuesFromFile
    {
        public string ServiceType { get; set; } = string.Empty;
        public string ServiceTypeInternational { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountNumberInternational { get; set; } = string.Empty;
        public uint ShipmentWeight { get; set; }
        public uint ShipmentLength { get; set; }
        public uint ShipmentWidth { get; set; }
        public uint ShipmentHeight { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderStreet { get; set; } = string.Empty;
        public string SenderStreetNumber { get; set; } = string.Empty;
        public string SenderCity { get; set; } = string.Empty;
        public string SenderZipCode { get; set; } = string.Empty;
        public string SenderCountryCode { get; set; } = string.Empty;
    }
}