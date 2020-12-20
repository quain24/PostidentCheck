using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    internal static class DhlResponseMessagesFixtures
    {
        public static Task<HttpResponseMessage> GetProperResponseFromRequest(HttpRequestMessage request)
        {
            try
            {
                var content = request.RequestUri.Query;
                content = content.Remove(content.IndexOf("?xml="), 5);

                content = Uri.UnescapeDataString(content);
                // XDocument.Load is better at correcting unseen bad chars from xml
                var byteArray = Encoding.ASCII.GetBytes(content);
                var stream = new MemoryStream(byteArray);

                var document = XDocument.Load(stream);
                var serializer = new XmlSerializer(typeof(DhlApiQueryFixtureModel));

                using var reader = document.CreateReader();
                var dto = serializer.Deserialize(reader) as DhlApiQueryFixtureModel;

                return Task.FromResult(GetProperHttpResponseMessageFrom(dto.ParcelTrackingNumbers.Split(new[] { ';' })));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Request passed in could not be deserialized, so it is likely in a wrong format", ex);
            }
        }

        public static Task<HttpResponseMessage> GetNotFoundResponsesFromRequest(HttpRequestMessage request)
        {
            try
            {
                var content = request.RequestUri.Query;
                content = content.Remove(content.IndexOf("?xml="), 5);

                content = Uri.UnescapeDataString(content);
                // XDocument.Load is better at correcting unseen bad chars from xml
                var byteArray = Encoding.ASCII.GetBytes(content);
                var stream = new MemoryStream(byteArray);

                var document = XDocument.Load(stream);
                var serializer = new XmlSerializer(typeof(DhlApiQueryFixtureModel));

                using var reader = document.CreateReader();
                var dto = serializer.Deserialize(reader) as DhlApiQueryFixtureModel;

                return Task.FromResult(GetNotFoundXMLResponse(dto.ParcelTrackingNumbers));
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Request passed in could not be deserialized, so it is likely in a wrong format", ex);
            }
        }

        internal static HttpResponseMessage GetProperHttpResponseMessageFrom(params string[] carrierTrackingNumbers)
        {
            var content = ProperStringContentFrom(carrierTrackingNumbers);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            };
            return httpResponse;
        }

        public static string ProperStringContentFrom(params string[] carrierTrackingNumbers)
        {
            var dtoResponse = new DhlResponseFixtureDto()
            {
                ErrorCode = 0,
                ErrorText = null,
                Name = "piece-shipment-list",
                RequestId = "fa4e5c87-9a81-408f-abff-c746f70539f3",
                Parcels = new List<DhlResponseParcelFixtureDto>()
            };

            foreach (var ctn in carrierTrackingNumbers)
            {
                dtoResponse.Parcels.Add(new DhlResponseParcelFixtureDto()
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
                    ShortStatus = "Delivery successful",
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

            var serializedDto = new DhlXmlParcelFixtureSerializer().Serialize(dtoResponse);
            return serializedDto;
        }

        internal static HttpResponseMessage GetForbiddenCodeResponse()
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("")
            };
            return httpResponse;
        }

        internal static HttpResponseMessage GetBadRequestCodeResponse()
        {
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("<!DOCTYPE html>\r\n<html>\r\n    <head>\r\n" +
                                            "        <title>CIG - Error report</title>\r\n    </head>\r\n    <body>\r\n" +
                                            "        <h1>HTTP Status 400 - </h1>\r\n        <div class=\"line\"></div>\r\n" +
                                            "        <p><b>type</b> Status report</p>\r\n        <p><b>message</b><u></u></p>\r\n" +
                                            "        <p><b>description</b><u>The request sent by the client was syntactically incorrect.</u></p>\r\n" +
                                            "        <div class=\"line\"/>\r\n        <h3>CIG</h3>\r\n    </body>\r\n</html>")
            };
            return httpResponse;
        }

        internal static HttpResponseMessage GetNotFoundXMLResponse(params string[] carrierTrackingNumbers)
        {
            var dto = new DhlResponseFixtureDto()
            {
                ErrorCode = 100,
                ErrorText = "No data found",
                Name = "piece-shipment-list",
                RequestId = "9371b255-485f-487b-8f6e-8bfa25c50a3b",
                Parcels = new List<DhlResponseParcelFixtureDto>()
            };

            foreach (var number in carrierTrackingNumbers)
            {
                dto.Parcels.Add(new DhlResponseParcelFixtureDto()
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

            var serializedDto = new DhlXmlParcelFixtureSerializer().Serialize(dto);

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(serializedDto)
            };
            return httpResponse;
        }
    }
}