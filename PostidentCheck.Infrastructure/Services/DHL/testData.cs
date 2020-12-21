﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services.DHL
{
    public static class testData
    {
        public static string xml()
        {
            return "<soapenv:Envelope xmlns:soapenv =\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:ns=\"http://dhl.de/webservices/businesscustomershipping/3.0\">   <soapenv:Header>      <cis:Authentification>         <cis:user>gartenundfreizeit</cis:user>         <cis:signature>!FuckFCB1900!</cis:signature>      </cis:Authentification>   </soapenv:Header><soapenv:Body>    <ns:ValidateShipmentOrderRequest>        <ns:Version>            <majorRelease>3</majorRelease>            <minorRelease>1</minorRelease>        </ns:Version>        <ShipmentOrder>            <sequenceNumber>123</sequenceNumber>            <Shipment>                <ShipmentDetails>                    <product>V01PAK</product>                    <cis:accountNumber>62203625500104</cis:accountNumber>                    <customerReference>Ref. 123456</customerReference>                    <shipmentDate>2020-12-29</shipmentDate>                    <costCentre></costCentre>                    <ShipmentItem>                        <weightInKG>5</weightInKG>                        <lengthInCM>60</lengthInCM>                        <widthInCM>30</widthInCM>                        <heightInCM>15</heightInCM>                    </ShipmentItem>                    <Service>                  </Service>                    <Notification>                        <recipientEmailAddress>empfaenger@test.de</recipientEmailAddress>                    </Notification>                </ShipmentDetails>                <Shipper>                    <Name>                        <cis:name1>Absender Zeile 1</cis:name1>                        <cis:name2>Absender Zeile 2</cis:name2>                        <cis:name3>Absender Zeile 3</cis:name3>                    </Name>                    <Address>                        <cis:streetName>Vegesacker Heerstr.</cis:streetName>                        <cis:streetNumber>111</cis:streetNumber>                        <cis:zip>28757</cis:zip>                        <cis:city>Bremen</cis:city>                        <cis:Origin>                            <cis:country></cis:country>                            <cis:countryISOCode>DE</cis:countryISOCode>                        </cis:Origin>                    </Address>                    <Communication>                        <!--Optional:-->                        <cis:phone>+49421987654321</cis:phone>                        <cis:email>absender@test.de</cis:email>                        <!--Optional:-->                        <cis:contactPerson>Kontaktperson Absender</cis:contactPerson>                    </Communication>                </Shipper>                <Receiver>                    <cis:name1>Empfänger Zeile 1</cis:name1>                    <Address>                        <cis:name2>Empfänger Zeile 2</cis:name2>                        <cis:name3>Empfänger Zeile 3</cis:name3>                        <cis:streetName>An der Weide</cis:streetName>                        <cis:streetNumber>50a</cis:streetNumber>                        <cis:zip>28195</cis:zip>                        <cis:city>Bremen</cis:city>                        <cis:Origin>                            <cis:country></cis:country>                            <cis:countryISOCode>DE</cis:countryISOCode>                        </cis:Origin>                    </Address>                    <Communication>                        <cis:phone>+49421123456789</cis:phone>                        <cis:email>empfaenger@test.de</cis:email>                        <cis:contactPerson>Kontaktperson Empfänger</cis:contactPerson>                    </Communication>                </Receiver>            </Shipment>            <PrintOnlyIfCodeable active=\"1\"/>        </ShipmentOrder>        <ShipmentOrder>            <sequenceNumber>321</sequenceNumber>            <Shipment>                <ShipmentDetails>                    <product>V01PAK</product>                    <cis:accountNumber>62203625500104</cis:accountNumber>                    <customerReference>Ref. 123456</customerReference>                    <shipmentDate>2020-12-29</shipmentDate>                    <costCentre></costCentre>                    <ShipmentItem>                        <weightInKG>5</weightInKG>                        <lengthInCM>60</lengthInCM>                        <widthInCM>30</widthInCM>                        <heightInCM>15</heightInCM>                    </ShipmentItem>                    <Service>                  </Service>                    <Notification>                        <recipientEmailAddress>empfaenger@test.de</recipientEmailAddress>                    </Notification>                </ShipmentDetails>                <Shipper>                    <Name>                        <cis:name1>Absender Zeile 1</cis:name1>                        <cis:name2>Absender Zeile 2</cis:name2>                        <cis:name3>Absender Zeile 3</cis:name3>                    </Name>                    <Address>                        <cis:streetName>Vegesacker Heerstr.</cis:streetName>                        <cis:streetNumber>111</cis:streetNumber>                        <cis:zip>28757</cis:zip>                        <cis:city>Bremen</cis:city>                        <cis:Origin>                            <cis:country></cis:country>                            <cis:countryISOCode>DE</cis:countryISOCode>                        </cis:Origin>                    </Address>                    <Communication>                        <!--Optional:-->                        <cis:phone>+49421987654321</cis:phone>                        <cis:email>absender@test.de</cis:email>                        <!--Optional:-->                        <cis:contactPerson>Kontaktperson Absender</cis:contactPerson>                    </Communication>                </Shipper>                <Receiver>                    <cis:name1>Empfänger Zeile 1</cis:name1>                    <Address>                        <cis:name2>Empfänger Zeile 2</cis:name2>                        <cis:name3>Empfänger Zeile 3</cis:name3>                        <cis:streetName>An der Weide</cis:streetName>                        <cis:streetNumber>50a</cis:streetNumber>                        <cis:zip>28195</cis:zip>                        <cis:city>Bremen</cis:city>                        <cis:Origin>                            <cis:country></cis:country>                            <cis:countryISOCode>DE</cis:countryISOCode>                        </cis:Origin>                    </Address>                    <Communication>                        <cis:phone>+49421123456789</cis:phone>                        <cis:email>empfaenger@test.de</cis:email>                        <cis:contactPerson>Kontaktperson Empfänger</cis:contactPerson>                    </Communication>                </Receiver>            </Shipment>            <PrintOnlyIfCodeable active=\"1\"/>        </ShipmentOrder>        <labelResponseType>URL</labelResponseType>        <groupProfileName></groupProfileName>        <labelFormat></labelFormat>        <labelFormatRetoure></labelFormatRetoure>        <combinedPrinting>0</combinedPrinting>    </ns:ValidateShipmentOrderRequest></soapenv:Body></soapenv:Envelope>";
                
        }
    }
}
