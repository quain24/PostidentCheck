using Postident.Core.Enums;
using Postident.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Postident.Infrastructure.Services.DHL
{
    public class ShipmentBuilder
    {
        private readonly ValidationXmlRequestBuilder _parentBuilder;
        private const string NotImplementedDeutschePostEndpointMsg = "Given endpoint has not been implemented in this object, cannot set up receiver data.";

        public ShipmentBuilder(XNamespace cisNamespace, ValidationXmlRequestBuilder parentBuilder, List<XElement> shipments)
        {
            _parentBuilder = parentBuilder;
            CisNamespace = cisNamespace;
            Shipments = shipments;
        }

        private XNamespace CisNamespace { get; }
        private List<XElement> Shipments { get; }
        private XElement SenderData { get; set; }
        private XElement ShipmentItem { get; set; }
        private XElement ReceiverData { get; set; }
        private XElement Id { get; set; }
        private XElement ShippingDate { get; set; }
        private XElement AccountNumber { get; set; }
        private XElement ServiceType { get; set; }

        /// <summary>
        /// Set up a unique <paramref name="id"/> that will enable this app to differentiate received responses.<br/>
        /// <paramref name="id"/> has to correspond to key column of data packs being checked.
        /// </summary>
        /// <param name="id">A key value from data being checked, unique</param>
        public ShipmentBuilder SetUpId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentOutOfRangeException(nameof(id),
                    "ID cannot be null or empty, it has to correspond to index of the order beeing checked");

            Id = new XElement("sequenceNumber", id);
            return this;
        }

        public ShipmentBuilder SetUpDhlServiceType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentOutOfRangeException(nameof(type),
                    "Type of service cannot be null or empty, it has to be one of proper DHL service types");

            ServiceType = new XElement("product", type);
            return this;
        }

        /// <summary>
        /// Theoretical shipping date - should be set to 'today' or a date in near future.
        /// </summary>
        /// <param name="date">Ship-out date of this shipment</param>
        public ShipmentBuilder SetUpShippingDate(DateTime date)
        {
            if (date.Date < DateTime.Today)
                throw new ArgumentOutOfRangeException(nameof(date),
                    "Given date needs to be current date or some future date.");

            ShippingDate = new XElement("shipmentDate", date.Date.ToString("yyyy-MM-dd"));

            return this;
        }

        /// <summary>
        /// Set up shippers account number.
        /// </summary>
        /// <param name="accountNumber">Shippers account number</param>
        public ShipmentBuilder SetUpAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentOutOfRangeException(nameof(accountNumber), "Account number cannot be null or empty.");

            AccountNumber = new XElement(CisNamespace + "accountNumber", accountNumber);

            return this;
        }

        /// <summary>
        /// Set up senders address and name data
        /// </summary>
        /// <param name="address"><see cref="Address"/> object containing senders address and naming data</param>
        public ShipmentBuilder SetUpSenderData(Address address)
        {
            SenderData = AddressGenerator(address, "Shipper");
            return this;
        }

        /// <summary>
        /// Set up receiver address and name data - MANDATORY
        /// </summary>
        /// <param name="address"><see cref="Address"/> object containing receivers address and naming data</param>
        public ShipmentBuilder SetUpReceiverData(Address address)
        {
            _ = address ?? throw new ArgumentNullException(nameof(address));

            ReceiverData = address.Street switch
            {
                "Packstation" => SetUpDeutschePostEndpointReceiverData(address, DeutschePostEndpointType.Packstation),
                "Postfiliale" => SetUpDeutschePostEndpointReceiverData(address, DeutschePostEndpointType.Postfiliale),
                _ => AddressGenerator(address, "Receiver")
            };
            return this;
        }

        private XElement AddressGenerator(Address address, string type)
        {
            var nameSchema = type.Equals("Shipper")
                ? new XElement("Name",
                    new XElement(CisNamespace + "name1", address.Name))
                : new XElement(CisNamespace + "name1", address.Name);

            return new(type,
                nameSchema,
                new XElement("Address",
                    new XElement(CisNamespace + "streetName", address.Street),
                    new XElement(CisNamespace + "streetNumber", address.StreetNumber),
                    new XElement(CisNamespace + "zip", address.ZipCode),
                    new XElement(CisNamespace + "city", address.City),
                    new XElement(CisNamespace + "Origin",
                        new XElement(CisNamespace + "countryISOCode", address.CountryCode)
                    )
                )
            );
        }

        private XElement SetUpDeutschePostEndpointReceiverData(Address address, DeutschePostEndpointType type)
        {
            if (!Enum.IsDefined(typeof(DeutschePostEndpointType), type))
            {
                throw new ArgumentOutOfRangeException(nameof(address),
                    $"A {nameof(type)} property of value {type}" +
                    " passed in is not defined in corresponding enum.");
            }

            return new XElement("Receiver",
                new XElement(CisNamespace + "name1", address.Name),
                new XElement(type switch
                {
                    DeutschePostEndpointType.Packstation => "Packstation",
                    DeutschePostEndpointType.Postfiliale => "Postfiliale",
                    _ => throw new ArgumentException(NotImplementedDeutschePostEndpointMsg)
                },
                    new XElement(CisNamespace + "postNumber", address.PostIdent),
                    new XElement(CisNamespace + type switch
                    {
                        DeutschePostEndpointType.Packstation => "packstationNumber",
                        DeutschePostEndpointType.Postfiliale => "postfilialNumber",
                        _ => throw new ArgumentException(NotImplementedDeutschePostEndpointMsg)
                    }, address.StreetNumber),
                    new XElement(CisNamespace + "zip", address.ZipCode),
                    new XElement(CisNamespace + "city", address.City),
                    new XElement(CisNamespace + "Origin",
                        new XElement(CisNamespace + "countryISOCode", address.CountryCode)
                    )
                )
            );
        }

        public ShipmentBuilder SetUpItemDimensions(uint weightInKg, uint lengthInCm, uint widthInCm,
            uint heightInCm)
        {
            CheckDimensions(weightInKg, lengthInCm, widthInCm, heightInCm);

            ShipmentItem = new XElement("ShipmentItem",
                new XElement("weightInKG", weightInKg),
                new XElement("lengthInCM", lengthInCm),
                new XElement("widthInCM", widthInCm),
                new XElement("heightInCM", heightInCm)
            );

            return this;
        }

        private static void CheckDimensions(uint weightInKg, uint lengthInCm, uint widthInCm,
            uint heightInCm)
        {
            var name = weightInKg == 0 ? nameof(weightInKg) :
                lengthInCm == 0 ? nameof(lengthInCm) :
                widthInCm == 0 ? nameof(widthInCm) :
                heightInCm == 0 ? nameof(heightInCm) :
                string.Empty;

            if (!string.IsNullOrEmpty(name)) throw new ArgumentNullException(name, $"Given {name} is 0. Every dimension must be a positive number!");
        }

        public ShipmentBuilder Reset()
        {
            Id = null;
            ReceiverData = null;
            SenderData = null;
            ShipmentItem = null;
            ShippingDate = null;
            AccountNumber = null;
            ServiceType = null;

            return this;
        }

        public ValidationXmlRequestBuilder BuildShipment()
        {
            if (Id is null)
                throw new MissingFieldException(nameof(ShipmentBuilder), nameof(Id));
            if (ReceiverData is null)
                throw new MissingFieldException(nameof(ShipmentBuilder), nameof(ReceiverData));

            if (SenderData is null)
            {
                SetUpSenderData(new Address()
                {
                    City = DefaultShipmentValues.SenderCity,
                    CountryCode = DefaultShipmentValues.SenderCountryCode,
                    Name = DefaultShipmentValues.SenderName,
                    Street = DefaultShipmentValues.SenderStreet,
                    StreetNumber = DefaultShipmentValues.SenderStreetNumber,
                    ZipCode = DefaultShipmentValues.SenderZipCode
                });
            }

            if (ShipmentItem is null)
            {
                SetUpItemDimensions
                (
                    DefaultShipmentValues.ShipmentWeight,
                    DefaultShipmentValues.ShipmentLength,
                    DefaultShipmentValues.ShipmentWidth,
                    DefaultShipmentValues.ShipmentHeight
                );
            }

            if (ShippingDate is null)
                SetUpShippingDate(DateTime.Today + TimeSpan.FromDays(1));

            if (AccountNumber is null)
                SetUpAccountNumber(DefaultShipmentValues.AccountNumber);

            if (ServiceType is null)
                SetUpDhlServiceType(DefaultShipmentValues.ServiceType);

            var shipment = new XElement("ShipmentOrder",
                Id,
                new XElement("Shipment",
                    new XElement("ShipmentDetails",
                        ServiceType,
                        AccountNumber,
                        ShippingDate,
                        ShipmentItem
                    ),
                    SenderData,
                    ReceiverData
                )
            );

            Shipments.Add(shipment);

            return _parentBuilder;
        }
    }
}