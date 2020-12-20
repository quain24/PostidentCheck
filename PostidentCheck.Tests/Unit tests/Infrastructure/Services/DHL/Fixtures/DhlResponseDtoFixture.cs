using System.Collections.Generic;
using Postident.Application.DHL;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class DhlResponseDtoFixture
    {
        public static DhlResponseDto ProperDhlResponseDtoFrom(string status, params string[] carrierTrackingNumbers)
        {
            var dtoResponse = new DhlResponseDto()
            {
                ErrorCode = 0,
                ErrorText = null,
                Name = "piece-shipment-list",
                RequestId = "fa4e5c87-9a81-408f-abff-c746f70539f3",
                Parcels = new List<DhlParcelResponseDto>()
            };

            foreach (var ctn in carrierTrackingNumbers)
            {
                dtoResponse.Parcels.Add(new DhlParcelResponseDto()
                {
                    Name = "piece-shipment",
                    ErrorCode = 0,
                    PieceId = "8e464a3e-219a-459b-823b-07d9d92732e3",
                    ShipmentCode = string.Empty,
                    PieceIdentifier = ctn[2..],
                    IdentifierType = 2,
                    PieceCode = ctn,
                    EventLocation = string.Empty,
                    EventCountry = "DE",
                    StatusList = "0",
                    StatusTimestamp = "18.03.2016 10:02",
                    Status = "The shipment has been successfully delivered",
                    ShortStatus = status,
                    RecipientName = "Kraemer",
                    RecipientStreet = "Heinrich-Brüning-Str. 7",
                    RecipientCity = "53113 Bonn",
                    PanRecipientName = "Deutsche Post DHL",
                    PanRecipientStreet = "Heinrich-Brüning-Str. 7",
                    PanRecipientCity = "53113 Bonn",
                    PanRecipientAddress = "Heinrich-Brüning-Str. 7 53113 Bonn",
                    PanRecipientPostalCode = "53113",
                    ShipperName = "No sender data has been transferred to DHL.",
                    ShipperStreet = string.Empty,
                    ShipperCity = string.Empty,
                    ShipperAddress = string.Empty,
                    ProductCode = "00",
                    ProductKey = string.Empty,
                    ProductName = "DHL PAKET (parcel)",
                    DeliveryEventFlag = "1",
                    RecipientId = "5",
                    RecipientIdText = "different person present",
                    Upu = string.Empty,
                    ShipmentLength = "0.0",
                    ShipmentWidth = "0.0",
                    ShipmentHeight = "0.0",
                    ShipmentWeight = "0.0",
                    InternationalFlag = "0",
                    Division = "DPEED",
                    Ice = "DLVRD",
                    Ric = "OTHER",
                    StandardEventCode = "ZU",
                    DestCountry = "DE",
                    OriginCountry = "DE",
                    SearchedPieceCode = ctn,
                    SearchedRefNo = string.Empty,
                    PieceCustomerReference = string.Empty,
                    LeitCode = string.Empty,
                    RoutingCodeEan = string.Empty,
                    Matchcode = string.Empty,
                    DomesticId = string.Empty,
                    AirwayBillNumber = string.Empty,
                    Ruecksendung = false,
                    PslzNr = "5066934803",
                    OrderPreferredDeliveryDay = false,
                    ShipmentCustomerReference = string.Empty
                });
            }

            return dtoResponse;
        }

        public static DhlResponseDto GetNotFoundDhlResponseDtoFrom(params string[] carrierTrackingNumbers)
        {
            var dto = new DhlResponseDto()
            {
                ErrorCode = 100,
                ErrorText = "No data found",
                Name = "piece-shipment-list",
                RequestId = "9371b255-485f-487b-8f6e-8bfa25c50a3b",
                Parcels = new List<DhlParcelResponseDto>()
            };

            foreach (var number in carrierTrackingNumbers)
            {
                dto.Parcels.Add(new DhlParcelResponseDto()
                {
                    Name = "piece-shipment",
                    SearchedPieceCode = number,
                    PieceCode = number,
                    PieceId = string.Empty,
                    RecipientName = string.Empty,
                    RecipientStreet = string.Empty,
                    RecipientCity = string.Empty,
                    Status = string.Empty,
                    ShortStatus = string.Empty,
                    DeliveryEventFlag = string.Empty,
                    Division = string.Empty,
                    RecipientId = string.Empty,
                    ProductCode = string.Empty,
                    ProductKey = string.Empty,
                    ProductName = string.Empty,
                    InternationalFlag = "0",
                    PieceStatus = "100",
                    PieceStatusDescription = "No data found"
                });
            }

            return dto;
        }
    }
}