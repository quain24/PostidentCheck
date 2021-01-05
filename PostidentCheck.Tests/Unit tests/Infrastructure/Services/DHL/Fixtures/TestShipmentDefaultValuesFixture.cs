using Postident.Infrastructure.Common;
using System.Collections.Generic;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class TestShipmentDefaultValuesFixture
    {
        public static DefaultShipmentValues Defaults()
        {
            return new DefaultShipmentValues(new DefaultShipmentValuesFromFile()
            {
                AccountNumber = "12345678901234",
                AccountNumberInternational = "43210987654321",
                ReceiverName = "Test receiver name",
                SenderCity = "Sender city name",
                SenderCountryCode = "de",
                SenderName = "Test sender name",
                SenderStreet = "Test sender street",
                SenderStreetNumber = "A1",
                SenderZipCode = "12345",
                ServiceType = "DHL01",
                ShipmentHeight = 1,
                ShipmentLength = 2,
                ShipmentWeight = 3,
                ShipmentWidth = 4,
                Amount = 1,
                CustomsValue = 1,
                Description = "Description",
                EuCountryCodes = new List<string>()
                {
                    "BE", "EL", "LT", "PT", "BG", "ES", "LU", "RO", "CZ", "FR", "HU", "SI", "DK", "HR", "MT", "SK",
                    "DE", "IT", "NL", "FI", "EE", "CY", "AT", "SE", "IE", "LV", "PL"
                },
                ExportType = "OTHER",
                ExportTypeDescription = "ExportTypeDescription",
                NetWeightInKG = 0.01,
                ServiceTypeInternational = "DHLINT"
            });
        }
    }
}