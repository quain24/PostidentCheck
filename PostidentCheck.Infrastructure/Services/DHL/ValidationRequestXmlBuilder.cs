using KeePass.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Postident.Infrastructure.Services.DHL
{
    public class ValidationRequestXmlBuilder : IValidationRequestXmlBuilder
    {
        private XNamespace SoapEnvNamespace { get; } = "http://schemas.xmlsoap.org/soap/envelope/";
        private XNamespace CisNamespace { get; } = "http://dhl.de/webservice/cisbase";
        private XNamespace NsNamespace { get; } = "http://dhl.de/webservices/businesscustomershipping/3.0";

        private XElement AuthorizationModule { get; set; }
        private XElement ApiVersionModule { get; set; }
        private List<XElement> Shipments { get; set; } = new();

        public IValidationRequestXmlBuilder SetUpAuthorization(Secret secret)
        {
            AuthorizationModule = new XElement(SoapEnvNamespace + "Header",
                new XElement(CisNamespace + "Authentification",
                    new XElement(CisNamespace + "user", secret.Username),
                    new XElement(CisNamespace + "signature", secret.Password)
                )
            );
            return this;
        }

        public IValidationRequestXmlBuilder SetUpApiVersion(uint majorRelease = 3, uint minorRelease = 1)
        {
            ApiVersionModule = new XElement(NsNamespace + "Version",
                new XElement("majorRelease", majorRelease),
                new XElement("minorRelease", minorRelease)
            );

            return this;
        }

        public ISingleShipmentBuilder AddNewShipment() => new SingleShipmentBuilder(CisNamespace, this, Shipments);

        public IValidationRequestXmlBuilder Reset()
        {
            AuthorizationModule = null;
            ApiVersionModule = null;
            Shipments = new List<XElement>();

            return this;
        }

        public string Build()
        {
            CheckValidity();

            if (ApiVersionModule is null)
                SetUpApiVersion();

            var soapRequest = new XDocument(
                new XDeclaration("1.0", "UTF-8", "no"),
                new XElement(SoapEnvNamespace + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soap", SoapEnvNamespace),
                    new XAttribute(XNamespace.Xmlns + "cis", CisNamespace),
                    new XAttribute(XNamespace.Xmlns + "ns", NsNamespace),
                    AuthorizationModule,
                    new XElement(SoapEnvNamespace + "Body",
                        new XElement(NsNamespace + "ValidateShipmentOrderRequest",
                            ApiVersionModule,
                            Shipments
                    )
                )
            ));

            // DHL specific encoding
            return soapRequest.ToString().Replace(";", "%3B");
        }

        private void CheckValidity()
        {
            if (AuthorizationModule is null)
                throw new MissingFieldException(nameof(ValidationRequestXmlBuilder), nameof(AuthorizationModule));

            if (Shipments.Count == 0)
                throw new MissingFieldException(nameof(ValidationRequestXmlBuilder), nameof(Shipments));
        }
    }
}