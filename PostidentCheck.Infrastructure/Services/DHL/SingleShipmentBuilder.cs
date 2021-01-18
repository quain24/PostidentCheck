using Postident.Application.Common.Models;
using Postident.Core.Enums;
using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Interfaces.DHL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Postident.Infrastructure.Services.DHL
{
    public class SingleShipmentBuilder : ISingleShipmentBuilder
    {
        private const string NotImplementedDeutschePostEndpointMsg = "Given endpoint has not been implemented in this object, cannot set up receiver data.";
        private readonly IDefaultShipmentValues _defaults;
        private readonly IValidationRequestXmlBuilder _parentBuilder;
        private string _senderCountryCode = string.Empty;
        private string _receiverCountryCode = string.Empty;

        internal SingleShipmentBuilder(string id, Address receiverAddress, IDefaultShipmentValues defaults, XNamespace cisNamespace, IValidationRequestXmlBuilder parentBuilder, IList<XElement> shipments)
        {
            _defaults = defaults;
            _parentBuilder = parentBuilder;
            CisNamespace = cisNamespace;
            Shipments = shipments;

            SetUpId(id);
            SetUpReceiverData(receiverAddress);
        }

        private XNamespace CisNamespace { get; }
        private IList<XElement> Shipments { get; }
        private XElement SenderData { get; set; }
        private XElement ShipmentItem { get; set; }
        private XElement ReceiverData { get; set; }
        private XElement Id { get; set; }
        private XElement ShippingDate { get; set; }
        private XElement AccountNumber { get; set; }
        private XElement ServiceType { get; set; }
        private XElement ExportDocument { get; set; }
        private XElement AdditionalService { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.SetUpId"/>
        /// </summary>
        /// <param name="id">A key value from data being checked, unique</param>
        public ISingleShipmentBuilder SetUpId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id),
                    "ID cannot be null or empty, it has to correspond to index of the order beeing checked");

            Id = new XElement("sequenceNumber", id);
            return this;
        }

        public ISingleShipmentBuilder SetUpDhlServiceType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentOutOfRangeException(nameof(type),
                    "Type of service cannot be null or empty, it has to be one of proper DHL service types");

            ServiceType = new XElement("product", type);
            return this;
        }

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.SetUpShippingDate"/>
        /// </summary>
        /// <param name="date">Ship-out date of this shipment</param>
        public ISingleShipmentBuilder SetUpShippingDate(DateTime date)
        {
            if (date.Date < DateTime.Today)
                throw new ArgumentOutOfRangeException(nameof(date),
                    "Given date needs to be current date or some future date.");

            ShippingDate = new XElement("shipmentDate", date.Date.ToString("yyyy-MM-dd"));

            return this;
        }

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.SetUpAccountNumber"/>
        /// </summary>
        /// <param name="accountNumber">Shippers account number</param>
        public ISingleShipmentBuilder SetUpAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentOutOfRangeException(nameof(accountNumber), "Account number cannot be null or empty.");

            AccountNumber = new XElement(CisNamespace + "accountNumber", accountNumber);

            return this;
        }

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.SetUpSenderData"/>
        /// </summary>
        /// <param name="address"><see cref="DataPack"/> object containing senders address and naming data</param>
        public ISingleShipmentBuilder SetUpSenderData(Address address)
        {
            SenderData = address is not null ? AddressGenerator(address, "Shipper") : throw new ArgumentNullException(nameof(address));
            _senderCountryCode = address.CountryCode;
            return this;
        }

        /// <summary>
        /// There is different schema for address element for when type of address is "Shipper" or not
        /// </summary>
        /// <param name="address"></param>
        /// <param name="type"></param>
        private XElement AddressGenerator(Address address, string type)
        {
            var nameSchema = type.Equals("Shipper")
                ? new XElement("Name",
                    new XElement(CisNamespace + "name1", address.Name))
                : new XElement(CisNamespace + "name1", address.Name);

            return new XElement(type,
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

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.SetUpReceiverData"/>
        /// </summary>
        /// <param name="address"><see cref="DataPack"/> object containing receivers address and naming data</param>
        public ISingleShipmentBuilder SetUpReceiverData(Address address)
        {
            _ = address ?? throw new ArgumentNullException(nameof(address));

            ReceiverData = address.Street switch
            {
                var o when o.Equals("Packstation", StringComparison.InvariantCultureIgnoreCase) => SetUpDeutschePostEndpointReceiverData(address, DeutschePostEndpointType.Packstation),
                var o when o.Equals("Postfiliale", StringComparison.InvariantCultureIgnoreCase) => SetUpDeutschePostEndpointReceiverData(address, DeutschePostEndpointType.Postfiliale),
                _ => AddressGenerator(address, "Receiver")
            };
            _receiverCountryCode = address.CountryCode;
            return this;
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

        public ISingleShipmentBuilder SetUpExportDocument(string type, string exportDescription, string description,
            string countryCode, uint amount, double netWeight, double value)
        {
            CheckExportValues(type, exportDescription, description, countryCode, amount, netWeight, value);

            ExportDocument = new XElement("ExportDocument",
                new XElement("exportType", type),
                new XElement("exportTypeDescription", exportDescription),
                new XElement("ExportDocPosition",
                    new XElement("description", description),
                    new XElement("countryCodeOrigin", countryCode),
                    new XElement("customsTariffNumber", "123456"),
                    new XElement("amount", amount),
                    new XElement("netWeightInKG", netWeight),
                    new XElement("customsValue", value)
                )
            );

            return this;
        }

        private static void CheckExportValues(string type, string exportDescription, string description,
            string countryCode, uint amount, double netWeight, double value)
        {
            var stringValueTest = new Dictionary<string, string>
            {
                {nameof(type), type}, {nameof(exportDescription), exportDescription}, {nameof(description), description}, {nameof(countryCode), countryCode}
            }.FirstOrDefault(kvp => string.IsNullOrEmpty(kvp.Value));

            if (!stringValueTest.Equals(default(KeyValuePair<string, string>)))
            {
                throw new ArgumentOutOfRangeException(stringValueTest.Key, "Given value cannot be null or empty");
            }

            if (amount == 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (netWeight <= 0) throw new ArgumentOutOfRangeException(nameof(netWeight));
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.SetUpItemDimensions"/>
        /// </summary>
        /// <param name="weightInKg">Shipment weight in kilograms</param>
        /// <param name="lengthInCm">Shipment length in centimeters</param>
        /// <param name="widthInCm">Shipment width in centimeters</param>
        /// <param name="heightInCm">Shipment height in centimeters</param>
        public ISingleShipmentBuilder SetUpItemDimensions(uint weightInKg, uint lengthInCm, uint widthInCm,
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

        public ISingleShipmentBuilder Reset()
        {
            Id = null;
            ReceiverData = null;
            _receiverCountryCode = string.Empty;
            SenderData = null;
            _senderCountryCode = string.Empty;
            ShipmentItem = null;
            ShippingDate = null;
            AccountNumber = null;
            ServiceType = null;
            ExportDocument = null;
            AdditionalService = null;

            return this;
        }

        /// <summary>
        /// <inheritdoc cref="ISingleShipmentBuilder.BuildShipment"/>
        /// </summary>
        /// <returns>Parent <see cref="IValidationRequestXmlBuilder"/> object</returns>
        /// <exception cref="MissingFieldException">If one or more required properties / fields are missing</exception>
        public IValidationRequestXmlBuilder BuildShipment()
        {
            CheckValidity();
            FillMissingOptionalFieldsWithDefaults();

            var shipment = new XElement("ShipmentOrder",
                Id,
                new XElement("Shipment",
                    new XElement("ShipmentDetails",
                        ServiceType,
                        AccountNumber,
                        ShippingDate,
                        ShipmentItem,
                        AdditionalService
                    ),
                    SenderData,
                    ReceiverData,
                    ExportDocument
                )
            );

            Shipments.Add(shipment);
            return _parentBuilder;
        }

        private void CheckValidity()
        {
            if (Id is null)
                throw new MissingFieldException(nameof(SingleShipmentBuilder), nameof(Id));
            if (ReceiverData is null)
                throw new MissingFieldException(nameof(SingleShipmentBuilder), nameof(ReceiverData));
        }

        private void FillMissingOptionalFieldsWithDefaults()
        {
            if (SenderData is null)
            {
                SetUpSenderData(new Address()
                {
                    City = _defaults.SenderCity,
                    CountryCode = _defaults.SenderCountryCode,
                    Name = _defaults.SenderName,
                    Street = _defaults.SenderStreet,
                    StreetNumber = _defaults.SenderStreetNumber,
                    ZipCode = _defaults.SenderZipCode
                });
            }

            if (ShipmentItem is null)
            {
                SetUpItemDimensions
                (
                    _defaults.ShipmentWeight,
                    _defaults.ShipmentLength,
                    _defaults.ShipmentWidth,
                    _defaults.ShipmentHeight
                );
            }

            if (ShippingDate is null)
                SetUpShippingDate(DateTime.Today + TimeSpan.FromDays(1));

            if (AccountNumber is null)
                SetUpAccountNumber(IsShipmentInternational() ? _defaults.AccountNumberInternational : _defaults.AccountNumber);

            if (ServiceType is null)
                SetUpDhlServiceType(IsShipmentInternational() ? _defaults.ServiceTypeInternational : _defaults.ServiceType);

            if (ExportDocument is null && IsShipmentInEu() is false)
                SetUpExportDocument(_defaults.ExportType, _defaults.ExportTypeDescription, _defaults.Description,
                    _senderCountryCode, _defaults.Amount, _defaults.NetWeightInKG, _defaults.CustomsValue);

            if (AdditionalService is null && IsShipmentInternational())
                SetUpAdditionalServices();
        }

        /// <summary>
        /// Will add additional service parameter to international shipping, so the api wont report</br>
        /// that a return policy / Service Vorausverfügung needs to be set for international addresses
        /// </summary>
        private ISingleShipmentBuilder SetUpAdditionalServices()
        {
            AdditionalService = new XElement("Service",
                new XElement("Endorsement", new XAttribute("active", "1"), new XAttribute("type", "IMMEDIATE"))
                );

            return this;
        }

        private bool IsShipmentInternational() =>
            string.Equals(_senderCountryCode, _receiverCountryCode, StringComparison.InvariantCultureIgnoreCase) is false;

        private bool IsShipmentInEu() =>
            _defaults.EuCountryCodes.Contains(_receiverCountryCode, StringComparer.InvariantCultureIgnoreCase);
    }
}