namespace Postident.Infrastructure.Common
{
    public static class DefaultShipmentValues
    {
        public static string ServiceType { get; set; }
        public static string AccountNumber { get; set; }
        public static uint ShipmentWeight { get; set; }
        public static uint ShipmentLength { get; set; }
        public static uint ShipmentWidth { get; set; }
        public static uint ShipmentHeight { get; set; }
        public static string ReceiverName { get; set; }
        public static string SenderName { get; set; }
        public static string SenderStreet { get; set; }
        public static string SenderStreetNumber { get; set; }
        public static string SenderCity { get; set; }
        public static string SenderZipCode { get; set; }
        public static string SenderCountryCode { get; set; }
    }
}