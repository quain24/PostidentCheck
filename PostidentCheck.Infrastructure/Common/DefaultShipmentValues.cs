using System;
using Postident.Infrastructure.Interfaces;

namespace Postident.Infrastructure.Common
{
    public class DefaultShipmentValues : IDefaultShipmentValues
    {
        internal DefaultShipmentValues(DefaultShipmentValuesFromFile values)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            SenderCity = values.SenderCity;
            AccountNumber = values.AccountNumber;
            ReceiverName = values.ReceiverName;
            SenderCountryCode = values.SenderCountryCode;
            SenderName = values.SenderName;
            SenderStreet = values.SenderStreet;
            SenderStreetNumber = values.SenderStreetNumber;
            SenderZipCode = values.SenderZipCode;
            ServiceType = values.ServiceType;
            ShipmentHeight = values.ShipmentHeight;
            ShipmentLength = values.ShipmentLength;
            ShipmentWidth = values.ShipmentWidth;
            ShipmentWeight = values.ShipmentWeight;
        }

        public string ServiceType { get; }
        public string AccountNumber { get; }
        public uint ShipmentWeight { get; }
        public uint ShipmentLength { get; }
        public uint ShipmentWidth { get; }
        public uint ShipmentHeight { get; }
        public string ReceiverName { get; }
        public string SenderName { get; }
        public string SenderStreet { get; }
        public string SenderStreetNumber { get; }
        public string SenderCity { get; }
        public string SenderZipCode { get; }
        public string SenderCountryCode { get; }
    }
}