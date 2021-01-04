using Postident.Infrastructure.Common;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class TestShipmentDefaultValuesFixture
    {
        public static DefaultShipmentValues Defaults()
        {
            return new DefaultShipmentValues(new DefaultShipmentValuesFromFile()
            {
                AccountNumber = "12345678901234",
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
                ShipmentWidth = 4
            });
        }
    }
}