namespace Postident.Infrastructure.Interfaces
{
    public interface IDefaultShipmentValues
    {
        string ServiceType { get; }
        string ServiceTypeInternational { get; }
        string AccountNumber { get; }
        string AccountNumberInternational { get; }
        uint ShipmentWeight { get; }
        uint ShipmentLength { get; }
        uint ShipmentWidth { get; }
        uint ShipmentHeight { get; }
        string ReceiverName { get; }
        string SenderName { get; }
        string SenderStreet { get; }
        string SenderStreetNumber { get; }
        string SenderCity { get; }
        string SenderZipCode { get; }
        string SenderCountryCode { get; }
    }
}