namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class DhlResponseFixtures
    {
        public static string ProperResponseSingleInfoString(string id = "1")
        {
            return $"<soap:Envelope xmlns:bcs=\"http://dhl.de/webservices/businesscustomershipping/3.0\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n    <soap:Header/>\r\n    <soap:Body>\r\n        <bcs:ValidateShipmentResponse>\r\n            <bcs:Version>\r\n                <majorRelease>3</majorRelease>\r\n                <minorRelease>0</minorRelease>\r\n            </bcs:Version>\r\n            <Status>\r\n                <statusCode>0</statusCode>\r\n                <statusText>ok</statusText>\r\n            </Status>\r\n            <ValidationState>\r\n                <sequenceNumber>{id}</sequenceNumber>\r\n                <Status>\r\n                    <statusCode>0</statusCode>\r\n                    <statusText>ok</statusText>\r\n                    <statusMessage>Der Webservice wurde ohne Fehler ausgef\u00FChrt.</statusMessage>\r\n                </Status>\r\n            </ValidationState>\r\n        </bcs:ValidateShipmentResponse>\r\n    </soap:Body>\r\n</soap:Envelope>";
        }

        public static string MajorXmlErrorResponse()
        {
            return "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n    <SOAP-ENV:Header/>\r\n    <SOAP-ENV:Body>\r\n        <SOAP-ENV:Fault>\r\n            <faultcode xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">env:Client</faultcode>\r\n            <faultstring>Invalid XML: cvc-length-valid: Wert '' mit L\u00E4nge = '0' ist nicht Facet-g\u00FCltig in Bezug auf die L\u00E4nge '14' f\u00FCr Typ '#AnonType_billingNumberAbstractShipmentType'.</faultstring>\r\n            <detail>\r\n                <ns7:ResponseStatus xmlns:ns7=\"http://shipment.webservice.vls.dhl.de/\" statusCode=\"11\" statusMessage=\"Das verwendete XML ist ung\u00FCltig.\" statusText=\"Not-Wellformed or invalid XML\"/>\r\n            </detail>\r\n        </SOAP-ENV:Fault>\r\n    </SOAP-ENV:Body>\r\n</SOAP-ENV:Envelope>";
        }

        public static string MajorXmlErrorCountry()
        {
            return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n    <soap:Body>\r\n        <soap:Fault>\r\n            <faultcode>soap:Server</faultcode>\r\n            <faultstring>Exception in extension function java.util.MissingResourceException: Couldn't find 3-letter country code for DEFF</faultstring>\r\n        </soap:Fault>\r\n    </soap:Body>\r\n</soap:Envelope>";
        }

        public static string WrongLoginResponse()
        {
            return "<soap:Envelope xmlns:bcs=\"http://dhl.de/webservices/businesscustomershipping/3.0\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n    <soap:Header/>\r\n    <soap:Body>\r\n        <bcs:ValidateShipmentResponse>\r\n            <bcs:Version>\r\n                <majorRelease>3</majorRelease>\r\n                <minorRelease>0</minorRelease>\r\n            </bcs:Version>\r\n            <Status>\r\n                <statusCode>118</statusCode>\r\n                <statusText>Invalid GKP username and/or password.</statusText>\r\n            </Status>\r\n        </bcs:ValidateShipmentResponse>\r\n    </soap:Body>\r\n</soap:Envelope>";
        }

        public static string ProperResponseDualInfoString()
        {
            return "<soap:Envelope xmlns:bcs=\"http://dhl.de/webservices/businesscustomershipping/3.0\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n    <soap:Header/>\r\n    <soap:Body>\r\n        <bcs:ValidateShipmentResponse>\r\n            <bcs:Version>\r\n                <majorRelease>3</majorRelease>\r\n                <minorRelease>0</minorRelease>\r\n            </bcs:Version>\r\n            <Status>\r\n                <statusCode>0</statusCode>\r\n                <statusText>ok</statusText>\r\n            </Status>\r\n            <ValidationState>\r\n                <sequenceNumber>123</sequenceNumber>\r\n                <Status>\r\n                    <statusCode>0</statusCode>\r\n                    <statusText>ok</statusText>\r\n                    <statusMessage>Der Webservice wurde ohne Fehler ausgef\u00FChrt.</statusMessage>\r\n                </Status>\r\n            </ValidationState>\r\n            <ValidationState>\r\n                <sequenceNumber>12345</sequenceNumber>\r\n                <Status>\r\n                    <statusCode>0</statusCode>\r\n                    <statusText>ok</statusText>\r\n                    <statusMessage>Der Webservice wurde ohne Fehler ausgef\u00FChrt.</statusMessage>\r\n                </Status>\r\n            </ValidationState>\r\n        </bcs:ValidateShipmentResponse>\r\n    </soap:Body>\r\n</soap:Envelope>";
        }

        public static string MinorErrorBadWeightAndCityResponse(string id = "1")
        {
            return $"<soap:Envelope xmlns:bcs=\"http://dhl.de/webservices/businesscustomershipping/3.0\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n    <soap:Header/>\r\n    <soap:Body>\r\n        <bcs:ValidateShipmentResponse>\r\n            <bcs:Version>\r\n                <majorRelease>3</majorRelease>\r\n                <minorRelease>0</minorRelease>\r\n            </bcs:Version>\r\n            <Status>\r\n                <statusCode>1101</statusCode>\r\n                <statusText>Hard validation error occured.</statusText>\r\n            </Status>\r\n            <ValidationState>\r\n                <sequenceNumber>{id}</sequenceNumber>\r\n                <Status>\r\n                    <statusCode>1101</statusCode>\r\n                    <statusText>Hard validation error occured.</statusText>\r\n                    <statusMessage>Die Gewichtsangabe ist zu hoch.</statusMessage>\r\n                    <statusMessage>Der Ort ist zu dieser PLZ nicht bekannt. Die Sendung ist trotzdem leitcodierbar und es wurde ein Versandschein erzeugt.</statusMessage>\r\n                    <statusMessage>Die Gewichtsangabe ist zu hoch.</statusMessage>\r\n                </Status>\r\n            </ValidationState>\r\n        </bcs:ValidateShipmentResponse>\r\n    </soap:Body>\r\n</soap:Envelope>";
        }
    }
}