﻿namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public static class RawXmlDataFixtures
    {
        public static string TestSingleShipmentA()
        {
            return "<ShipmentOrder>\r\n  <sequenceNumber>123</sequenceNumber>\r\n  <Shipment>\r\n    <ShipmentDetails>\r\n      <product>DHL01</product>\r\n      <accountNumber xmlns=\"ns\">12345678901234</accountNumber>\r\n      <shipmentDate>3000-01-01</shipmentDate>\r\n      <ShipmentItem>\r\n        <weightInKG>3</weightInKG>\r\n        <lengthInCM>2</lengthInCM>\r\n        <widthInCM>4</widthInCM>\r\n        <heightInCM>1</heightInCM>\r\n      </ShipmentItem>\r\n    </ShipmentDetails>\r\n    <Shipper>\r\n      <Name>\r\n        <name1 xmlns=\"ns\">Test sender name</name1>\r\n      </Name>\r\n      <Address>\r\n        <streetName xmlns=\"ns\">Test sender street</streetName>\r\n        <streetNumber xmlns=\"ns\">A1</streetNumber>\r\n        <zip xmlns=\"ns\">12345</zip>\r\n        <city xmlns=\"ns\">Sender city name</city>\r\n        <Origin xmlns=\"ns\">\r\n          <countryISOCode>de</countryISOCode>\r\n        </Origin>\r\n      </Address>\r\n    </Shipper>\r\n    <Receiver>\r\n      <name1 xmlns=\"ns\">a</name1>\r\n      <Address>\r\n        <streetName xmlns=\"ns\">a</streetName>\r\n        <streetNumber xmlns=\"ns\">1</streetNumber>\r\n        <zip xmlns=\"ns\">12345</zip>\r\n        <city xmlns=\"ns\">a</city>\r\n        <Origin xmlns=\"ns\">\r\n          <countryISOCode>de</countryISOCode>\r\n        </Origin>\r\n      </Address>\r\n    </Receiver>\r\n  </Shipment>\r\n</ShipmentOrder>";
        }

        public static string TestSingleShipmentB()
        {
            return "<ShipmentOrder>\r\n  <sequenceNumber>456</sequenceNumber>\r\n  <Shipment>\r\n    <ShipmentDetails>\r\n      <product>DHL01</product>\r\n      <accountNumber xmlns=\"ns\">12345678901234</accountNumber>\r\n      <shipmentDate>3000-01-01</shipmentDate>\r\n      <ShipmentItem>\r\n        <weightInKG>3</weightInKG>\r\n        <lengthInCM>2</lengthInCM>\r\n        <widthInCM>4</widthInCM>\r\n        <heightInCM>1</heightInCM>\r\n      </ShipmentItem>\r\n    </ShipmentDetails>\r\n    <Shipper>\r\n      <Name>\r\n        <name1 xmlns=\"ns\">Test sender name</name1>\r\n      </Name>\r\n      <Address>\r\n        <streetName xmlns=\"ns\">Test sender street</streetName>\r\n        <streetNumber xmlns=\"ns\">A1</streetNumber>\r\n        <zip xmlns=\"ns\">12345</zip>\r\n        <city xmlns=\"ns\">Sender city name</city>\r\n        <Origin xmlns=\"ns\">\r\n          <countryISOCode>de</countryISOCode>\r\n        </Origin>\r\n      </Address>\r\n    </Shipper>\r\n    <Receiver>\r\n      <name1 xmlns=\"ns\">b</name1>\r\n      <Address>\r\n        <streetName xmlns=\"ns\">b</streetName>\r\n        <streetNumber xmlns=\"ns\">1</streetNumber>\r\n        <zip xmlns=\"ns\">12345</zip>\r\n        <city xmlns=\"ns\">b</city>\r\n        <Origin xmlns=\"ns\">\r\n          <countryISOCode>de</countryISOCode>\r\n        </Origin>\r\n      </Address>\r\n    </Receiver>\r\n  </Shipment>\r\n</ShipmentOrder>";
        }

        public static string TestSingleInternationalShipment()
        {
            return "<ShipmentOrder>\r\n  <sequenceNumber>123</sequenceNumber>\r\n  <Shipment>\r\n    <ShipmentDetails>\r\n      <product>DHLINT</product>\r\n      <accountNumber xmlns=\"ns\">43210987654321</accountNumber>\r\n      <shipmentDate>3000-01-01</shipmentDate>\r\n      <ShipmentItem>\r\n        <weightInKG>3</weightInKG>\r\n        <lengthInCM>2</lengthInCM>\r\n        <widthInCM>4</widthInCM>\r\n        <heightInCM>1</heightInCM>\r\n      </ShipmentItem>\r\n      <Service>\r\n        <Endorsement active=\"1\" type=\"IMMEDIATE\" />\r\n      </Service>\r\n    </ShipmentDetails>\r\n    <Shipper>\r\n      <Name>\r\n        <name1 xmlns=\"ns\">Test sender name</name1>\r\n      </Name>\r\n      <Address>\r\n        <streetName xmlns=\"ns\">Test sender street</streetName>\r\n        <streetNumber xmlns=\"ns\">A1</streetNumber>\r\n        <zip xmlns=\"ns\">12345</zip>\r\n        <city xmlns=\"ns\">Sender city name</city>\r\n        <Origin xmlns=\"ns\">\r\n          <countryISOCode>de</countryISOCode>\r\n        </Origin>\r\n      </Address>\r\n    </Shipper>\r\n    <Receiver>\r\n      <name1 xmlns=\"ns\">a</name1>\r\n      <Address>\r\n        <streetName xmlns=\"ns\">a</streetName>\r\n        <streetNumber xmlns=\"ns\">1</streetNumber>\r\n        <zip xmlns=\"ns\">12345</zip>\r\n        <city xmlns=\"ns\">a</city>\r\n        <Origin xmlns=\"ns\">\r\n          <countryISOCode>at</countryISOCode>\r\n        </Origin>\r\n      </Address>\r\n    </Receiver>\r\n  </Shipment>\r\n</ShipmentOrder>";
        }

        public static string TestValidationXmlDomesticRequest()
        {
            return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:ns=\"http://dhl.de/webservices/businesscustomershipping/3.0\">\r\n  <soap:Header>\r\n    <cis:Authentification>\r\n      <cis:user>usr</cis:user>\r\n      <cis:signature>pass</cis:signature>\r\n    </cis:Authentification>\r\n  </soap:Header>\r\n  <soap:Body>\r\n    <ns:ValidateShipmentOrderRequest>\r\n      <ns:Version>\r\n        <majorRelease>3</majorRelease>\r\n        <minorRelease>1</minorRelease>\r\n      </ns:Version>\r\n      <ShipmentOrder>\r\n        <sequenceNumber>123</sequenceNumber>\r\n        <Shipment>\r\n          <ShipmentDetails>\r\n            <product>DHL01</product>\r\n            <cis:accountNumber>12345678901234</cis:accountNumber>\r\n            <shipmentDate>3000-01-01</shipmentDate>\r\n            <ShipmentItem>\r\n              <weightInKG>3</weightInKG>\r\n              <lengthInCM>2</lengthInCM>\r\n              <widthInCM>4</widthInCM>\r\n              <heightInCM>1</heightInCM>\r\n            </ShipmentItem>\r\n          </ShipmentDetails>\r\n          <Shipper>\r\n            <Name>\r\n              <cis:name1>Test sender name</cis:name1>\r\n            </Name>\r\n            <Address>\r\n              <cis:streetName>Test sender street</cis:streetName>\r\n              <cis:streetNumber>A1</cis:streetNumber>\r\n              <cis:zip>12345</cis:zip>\r\n              <cis:city>Sender city name</cis:city>\r\n              <cis:Origin>\r\n                <cis:countryISOCode>de</cis:countryISOCode>\r\n              </cis:Origin>\r\n            </Address>\r\n          </Shipper>\r\n          <Receiver>\r\n            <cis:name1>a</cis:name1>\r\n            <Address>\r\n              <cis:streetName>a</cis:streetName>\r\n              <cis:streetNumber>1</cis:streetNumber>\r\n              <cis:zip>12345</cis:zip>\r\n              <cis:city>a</cis:city>\r\n              <cis:Origin>\r\n                <cis:countryISOCode>de</cis:countryISOCode>\r\n              </cis:Origin>\r\n            </Address>\r\n          </Receiver>\r\n        </Shipment>\r\n      </ShipmentOrder>\r\n    </ns:ValidateShipmentOrderRequest>\r\n  </soap:Body>\r\n</soap:Envelope>";
        }

        public static string TestValidationXmlForeignInEuRequest()
        {
            return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:ns=\"http://dhl.de/webservices/businesscustomershipping/3.0\">\r\n  <soap:Header>\r\n    <cis:Authentification>\r\n      <cis:user>usr</cis:user>\r\n      <cis:signature>pass</cis:signature>\r\n    </cis:Authentification>\r\n  </soap:Header>\r\n  <soap:Body>\r\n    <ns:ValidateShipmentOrderRequest>\r\n      <ns:Version>\r\n        <majorRelease>3</majorRelease>\r\n        <minorRelease>1</minorRelease>\r\n      </ns:Version>\r\n      <ShipmentOrder>\r\n        <sequenceNumber>123</sequenceNumber>\r\n        <Shipment>\r\n          <ShipmentDetails>\r\n            <product>DHLINT</product>\r\n            <cis:accountNumber>43210987654321</cis:accountNumber>\r\n            <shipmentDate>3000-01-01</shipmentDate>\r\n            <ShipmentItem>\r\n              <weightInKG>3</weightInKG>\r\n              <lengthInCM>2</lengthInCM>\r\n              <widthInCM>4</widthInCM>\r\n              <heightInCM>1</heightInCM>\r\n            </ShipmentItem>\r\n            <Service>\r\n              <Endorsement active=\"1\" type=\"IMMEDIATE\" />\r\n            </Service>\r\n          </ShipmentDetails>\r\n          <Shipper>\r\n            <Name>\r\n              <cis:name1>Test sender name</cis:name1>\r\n            </Name>\r\n            <Address>\r\n              <cis:streetName>Test sender street</cis:streetName>\r\n              <cis:streetNumber>A1</cis:streetNumber>\r\n              <cis:zip>12345</cis:zip>\r\n              <cis:city>Sender city name</cis:city>\r\n              <cis:Origin>\r\n                <cis:countryISOCode>de</cis:countryISOCode>\r\n              </cis:Origin>\r\n            </Address>\r\n          </Shipper>\r\n          <Receiver>\r\n            <cis:name1>a</cis:name1>\r\n            <Address>\r\n              <cis:streetName>a</cis:streetName>\r\n              <cis:streetNumber>1</cis:streetNumber>\r\n              <cis:zip>12345</cis:zip>\r\n              <cis:city>a</cis:city>\r\n              <cis:Origin>\r\n                <cis:countryISOCode>AT</cis:countryISOCode>\r\n              </cis:Origin>\r\n            </Address>\r\n          </Receiver>\r\n        </Shipment>\r\n      </ShipmentOrder>\r\n    </ns:ValidateShipmentOrderRequest>\r\n  </soap:Body>\r\n</soap:Envelope>";
        }

        public static string TestValidationXmlForeignOutsideEu()
        {
            return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:ns=\"http://dhl.de/webservices/businesscustomershipping/3.0\">\r\n  <soap:Header>\r\n    <cis:Authentification>\r\n      <cis:user>usr</cis:user>\r\n      <cis:signature>pass</cis:signature>\r\n    </cis:Authentification>\r\n  </soap:Header>\r\n  <soap:Body>\r\n    <ns:ValidateShipmentOrderRequest>\r\n      <ns:Version>\r\n        <majorRelease>3</majorRelease>\r\n        <minorRelease>1</minorRelease>\r\n      </ns:Version>\r\n      <ShipmentOrder>\r\n        <sequenceNumber>123</sequenceNumber>\r\n        <Shipment>\r\n          <ShipmentDetails>\r\n            <product>DHLINT</product>\r\n            <cis:accountNumber>43210987654321</cis:accountNumber>\r\n            <shipmentDate>3000-01-01</shipmentDate>\r\n            <ShipmentItem>\r\n              <weightInKG>3</weightInKG>\r\n              <lengthInCM>2</lengthInCM>\r\n              <widthInCM>4</widthInCM>\r\n              <heightInCM>1</heightInCM>\r\n            </ShipmentItem>\r\n            <Service>\r\n              <Endorsement active=\"1\" type=\"IMMEDIATE\" />\r\n            </Service>\r\n          </ShipmentDetails>\r\n          <Shipper>\r\n            <Name>\r\n              <cis:name1>Test sender name</cis:name1>\r\n            </Name>\r\n            <Address>\r\n              <cis:streetName>Test sender street</cis:streetName>\r\n              <cis:streetNumber>A1</cis:streetNumber>\r\n              <cis:zip>12345</cis:zip>\r\n              <cis:city>Sender city name</cis:city>\r\n              <cis:Origin>\r\n                <cis:countryISOCode>de</cis:countryISOCode>\r\n              </cis:Origin>\r\n            </Address>\r\n          </Shipper>\r\n          <Receiver>\r\n            <cis:name1>a</cis:name1>\r\n            <Address>\r\n              <cis:streetName>a</cis:streetName>\r\n              <cis:streetNumber>1</cis:streetNumber>\r\n              <cis:zip>12345</cis:zip>\r\n              <cis:city>a</cis:city>\r\n              <cis:Origin>\r\n                <cis:countryISOCode>CN</cis:countryISOCode>\r\n              </cis:Origin>\r\n            </Address>\r\n          </Receiver>\r\n          <ExportDocument>\r\n            <exportType>OTHER</exportType>\r\n            <exportTypeDescription>ExportTypeDescription</exportTypeDescription>\r\n            <ExportDocPosition>\r\n              <description>Description</description>\r\n              <countryCodeOrigin>de</countryCodeOrigin>\r\n              <customsTariffNumber>123456</customsTariffNumber>\r\n              <amount>1</amount>\r\n              <netWeightInKG>0.01</netWeightInKG>\r\n              <customsValue>1</customsValue>\r\n            </ExportDocPosition>\r\n          </ExportDocument>\r\n        </Shipment>\r\n      </ShipmentOrder>\r\n    </ns:ValidateShipmentOrderRequest>\r\n  </soap:Body>\r\n</soap:Envelope>";
        }
    }
}