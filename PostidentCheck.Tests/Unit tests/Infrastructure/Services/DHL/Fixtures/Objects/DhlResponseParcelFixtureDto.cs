using System.Xml.Serialization;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures.Objects
{
    public class DhlResponseParcelFixtureDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("error-status")]
        public int ErrorCode { get; set; }

        [XmlAttribute("piece-id")]
        public string PieceId { get; set; }

        [XmlAttribute("shipment-code")]
        public string ShipmentCode { get; set; }

        [XmlAttribute("piece-identifier")]
        public string PieceIdentifier { get; set; }

        [XmlAttribute("identifier-type")]
        public int IdentifierType { get; set; }

        [XmlAttribute("piece-code")]
        public string PieceCode { get; set; }

        [XmlAttribute("event-location")]
        public string EventLocation { get; set; }

        [XmlAttribute("event-country")]
        public string EventCountry { get; set; }

        [XmlAttribute("status-liste")]
        public string StatusList { get; set; }

        [XmlAttribute("status-timestamp")]
        public string StatusTimestamp { get; set; }

        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("short-status")]
        public string ShortStatus { get; set; }

        [XmlAttribute("recipient-name")]
        public string RecipientName { get; set; }

        [XmlAttribute("recipient-street")]
        public string RecipientStreet { get; set; }

        [XmlAttribute("recipient-city")]
        public string RecipientCity { get; set; }

        [XmlAttribute("pan-recipient-name")]
        public string PanRecipientName { get; set; }

        [XmlAttribute("pan-recipient-street")]
        public string PanRecipientStreet { get; set; }

        [XmlAttribute("pan-recipient-city")]
        public string PanRecipientCity { get; set; }

        [XmlAttribute("pan-recipient-address")]
        public string PanRecipientAddress { get; set; }

        [XmlAttribute("pan-recipient-postalcode")]
        public string PanRecipientPostalCode { get; set; }

        [XmlAttribute("shipper-name")]
        public string ShipperName { get; set; }

        [XmlAttribute("shipper-street")]
        public string ShipperStreet { get; set; }

        [XmlAttribute("shipper-city")]
        public string ShipperCity { get; set; }

        [XmlAttribute("shipper-address")]
        public string ShipperAddress { get; set; }

        [XmlAttribute("product-code")]
        public string ProductCode { get; set; }

        [XmlAttribute("product-key")]
        public string ProductKey { get; set; }

        [XmlAttribute("product-name")]
        public string ProductName { get; set; }

        [XmlAttribute("delivery-event-flag")]
        public string DeliveryEventFlag { get; set; }

        [XmlAttribute("recipient-id")]
        public string RecipientId { get; set; }

        [XmlAttribute("recipient-id-text")]
        public string RecipientIdText { get; set; }

        [XmlAttribute("upu")]
        public string Upu { get; set; }

        [XmlAttribute("shipment-length")]
        public string ShipmentLength { get; set; }

        [XmlAttribute("shipment-width")]
        public string ShipmentWidth { get; set; }

        [XmlAttribute("shipment-height")]
        public string ShipmentHeight { get; set; }

        [XmlAttribute("shipment-weight")]
        public string ShipmentWeight { get; set; }

        [XmlAttribute("international-flag")]
        public string InternationalFlag { get; set; }

        [XmlAttribute("division")]
        public string Division { get; set; }

        [XmlAttribute("ice")]
        public string Ice { get; set; }

        [XmlAttribute("ric")]
        public string Ric { get; set; }

        [XmlAttribute("standard-event-code")]
        public string StandardEventCode { get; set; }

        [XmlAttribute("dest-country")]
        public string DestCountry { get; set; }

        [XmlAttribute("origin-country")]
        public string OriginCountry { get; set; }

        [XmlAttribute("searched-piece-code")]
        public string SearchedPieceCode { get; set; }

        [XmlAttribute("piece-status")]
        public string PieceStatus { get; set; }

        [XmlAttribute("piece-status-desc")]
        public string PieceStatusDescription { get; set; }

        [XmlAttribute("searched-ref-no")]
        public string SearchedRefNo { get; set; }

        [XmlAttribute("piece-customer-reference")]
        public string PieceCustomerReference { get; set; }

        [XmlAttribute("shipment-customer-reference")]
        public string ShipmentCustomerReference { get; set; }

        [XmlAttribute("leitcode")]
        public string LeitCode { get; set; }

        [XmlAttribute("routing-code-ean")]
        public string RoutingCodeEan { get; set; }

        [XmlAttribute("matchcode")]
        public string Matchcode { get; set; }

        [XmlAttribute("domestic-id")]
        public string DomesticId { get; set; }

        [XmlAttribute("airway-bill-number")]
        public string AirwayBillNumber { get; set; }

        [XmlAttribute("ruecksendung")]
        public bool Ruecksendung { get; set; }

        [XmlAttribute("pslz-nr")]
        public string PslzNr { get; set; }

        [XmlAttribute("order-preferred-delivery-day")]
        public bool OrderPreferredDeliveryDay { get; set; }
    }
}