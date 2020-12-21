using System;
using System.Xml.Linq;
using KeePass.Models;

namespace Postident.Infrastructure.Services.DHL
{
    public class ValidationXmlRequestBuilder
    {
        private XNamespace SoapEnvNamespace { get; } = "http://schemas.xmlsoap.org/soap/envelope/";
        private XNamespace CisNamespace { get; } = "http://dhl.de/webservice/cisbase";
        private XNamespace NsNamespace { get; } = "http://dhl.de/webservices/businesscustomershipping/3.0";

        private XElement AuthorizationModule { get; set; }

        public ValidationXmlRequestBuilder SetUpAuthorization(Secret secret)
        {
            AuthorizationModule = new XElement(CisNamespace + "Authentification",
                new XElement(CisNamespace + "user", secret.Username),
                new XElement(CisNamespace + "signature", secret.Password)
            );
            return this;
        }
        
        
        
        public string Build()
        {
            throw new NotImplementedException();
        }
    }
}