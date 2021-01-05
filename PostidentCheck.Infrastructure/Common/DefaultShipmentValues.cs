using Postident.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Postident.Infrastructure.Common
{
    public class DefaultShipmentValues : IDefaultShipmentValues
    {
        internal DefaultShipmentValues(DefaultShipmentValuesFromFile values)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            SenderCity = values.SenderCity;
            AccountNumber = values.AccountNumber;
            AccountNumberInternational = values.AccountNumberInternational;

            ReceiverName = values.ReceiverName;
            SenderCountryCode = values.SenderCountryCode;
            SenderName = values.SenderName;
            SenderStreet = values.SenderStreet;
            SenderStreetNumber = values.SenderStreetNumber;
            SenderZipCode = values.SenderZipCode;
            ServiceType = values.ServiceType;
            ServiceTypeInternational = values.ServiceTypeInternational;
            ShipmentHeight = values.ShipmentHeight;
            ShipmentLength = values.ShipmentLength;
            ShipmentWidth = values.ShipmentWidth;
            ShipmentWeight = values.ShipmentWeight;
            ExportType = values.ExportType;
            ExportTypeDescription = values.ExportTypeDescription;
            Description = values.Description;
            Amount = values.Amount;
            NetWeightInKG = values.NetWeightInKG;
            CustomsValue = values.CustomsValue;
            EuCountryCodes = values.EuCountryCodes;
        }

        public string ServiceType { get; }
        public string ServiceTypeInternational { get; }
        public string AccountNumber { get; }
        public string AccountNumberInternational { get; }
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
        public string ExportType { get; }
        public string ExportTypeDescription { get; }
        public string Description { get; }
        public uint Amount { get; }
        public double NetWeightInKG { get; }
        public double CustomsValue { get; }
        public List<string> EuCountryCodes { get; }
    }
}