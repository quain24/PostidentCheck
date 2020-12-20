using DHLRef;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services
{
    public class TestService
    {
        public async Task test()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://cig.dhl.de/services/production/soap/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            client.DefaultRequestHeaders.Add("SOAPAction", "\"urn:validateShipment\"");

            var byteArray = Encoding.ASCII.GetBytes(string.Join(':', "App_1", "lpRsWeabLuQNUlCRxj76KfQPorbjGJ"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var message = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"" +
                          " xmlns:cis=\"http://dhl.de/webservice/cisbase\" xmlns:ns=\"http://dhl.de/webservices/businesscustomershipping/3.0\">\r\n" +
                          "   <soapenv:Header>\r\n      <cis:Authentification>\r\n         <cis:user>gartenundfreizeit</cis:user>\r\n" +
                          "         <cis:signature>!Lotus125!</cis:signature>\r\n      </cis:Authentification>\r\n   </soapenv:Header>\r\n" +
                          "   <soapenv:Body>\r\n      <ns:ValidateShipmentOrderRequest>\r\n         <ns:Version>\r\n            <majorRelease>3</majorRelease>\r\n" +
                          "            <minorRelease>1</minorRelease>\r\n         </ns:Version>\r\n         <ShipmentOrder>\r\n            <Shipment>\r\n" +
                          "               <ShipmentDetails>\r\n                  <product>V01PAK</product>\r\n" +
                          "                  <cis:accountNumber>62203625500104</cis:accountNumber>\r\n" +
                          "                  <shipmentDate>2020-12-29</shipmentDate>\r\n                  <ShipmentItem>\r\n" +
                          "                     <weightInKG>5</weightInKG>\r\n                     <lengthInCM>60</lengthInCM>\r\n" +
                          "                     <widthInCM>30</widthInCM>\r\n                     <heightInCM>15</heightInCM>\r\n" +
                          "                  </ShipmentItem>\r\n               </ShipmentDetails>\r\n               <Shipper>\r\n" +
                          "                  <Name>\r\n                     <cis:name1>Absender Zeile 1</cis:name1>\r\n" +
                          "                  </Name>\r\n                  <Address>\r\n                     <cis:streetName>Vegesacker Heerstr.</cis:streetName>\r\n" +
                          "                     <cis:streetNumber>111</cis:streetNumber>\r\n                     <cis:zip>28757</cis:zip>\r\n" +
                          "                     <cis:city>Bremen</cis:city>\r\n                     <cis:Origin>\r\n" +
                          "                        <cis:countryISOCode>DE</cis:countryISOCode>\r\n                     </cis:Origin>\r\n" +
                          "                  </Address>\r\n               </Shipper>\r\n              <Receiver>\r\n " +
                          "                 <cis:name1>Empf\u00E4nger Zeile 1</cis:name1>\r\n                  <Address>\r\n " +
                          "                    <cis:Origin>\r\n                        <cis:countryISOCode>DE</cis:countryISOCode>\r\n" +
                          "                     </cis:Origin>\r\n                  </Address>\r\n                  <Packstation>\r\n " +
                          "                    <cis:postNumber>960873749</cis:postNumber>\r\n                     <cis:packstationNumber>102</cis:packstationNumber>\r\n" +
                          "                     <cis:zip>28195</cis:zip>\r\n                     <cis:city>Bremen</cis:city>\r\n" +
                          "                     <cis:Origin>\r\n                        <cis:countryISOCode>DE</cis:countryISOCode>\r\n " +
                          "                    </cis:Origin>\r\n                  </Packstation>\r\n               </Receiver>\r\n " +
                          "           </Shipment>\r\n            <PrintOnlyIfCodeable active=\"1\"/>\r\n         </ShipmentOrder>\r\n" +
                          "      </ns:ValidateShipmentOrderRequest>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>";

            var request = new HttpRequestMessage(HttpMethod.Post, "") { Content = new StringContent(message) };
            

            var response = await client.SendAsync(request);

            var res = await response.Content.ReadAsStringAsync();

            var t = "";
        }

        public async Task testNew()
        {

            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            var client = new GKVAPIServicePortTypeClient(GKVAPIServicePortTypeClient.EndpointConfiguration.GKVAPISOAP11port0);
            client.ClientCredentials.UserName.UserName = "App_1"; //"postident_validator_1";
            client.ClientCredentials.UserName.Password = "lpRsWeabLuQNUlCRxj76KfQPorbjGJ";//"4H4lnXBqGDS26xrLw4FytmvnzsDTXi";
            client.Endpoint.Binding = binding;

            var auth = new AuthentificationType();
            auth.user = "gartenundfreizeit";
            auth.signature = "!Lotus125!";

            var response = await client.validateShipmentAsync(auth, new ValidateShipmentOrderRequest()
            {
                ShipmentOrder = new ValidateShipmentOrderType[1]
                {
                    new ValidateShipmentOrderType()
                    {
                        PrintOnlyIfCodeable = new Serviceconfiguration()
                        {
                            active = ServiceconfigurationActive.Item1
                        },
                        Shipment = new ValidateShipmentOrderTypeShipment()
                        {
                            ShipmentDetails = new ShipmentDetailsTypeType()
                            {
                                product = "V01PAK",
                                accountNumber = "62203625500104",
                                shipmentDate = "2020-12-29",
                                ShipmentItem = new ShipmentItemType()
                                {
                                    heightInCM = "10",
                                    lengthInCM = "10",
                                    widthInCM = "10",
                                    weightInKG = 5
                                }
                            },
                            Shipper = new ShipperType()
                            {
                                Name = new NameType()
                                {
                                    name1 = "normal name"
                                },
                                Address = new NativeAddressType()
                                {
                                    Origin = new CountryType()
                                    {
                                        countryISOCode = "DE"
                                    },
                                    city = "Bremen",
                                    streetName = "Vegesacker Heerstr.",
                                    streetNumber = "111",
                                    zip = "28757"
                                }
                            },
                            Receiver = new ReceiverType()
                            {
                                name1 = "A receiver name",
                                Item = new PostfilialeType()
                                {
                                    city = "Bremen",
                                    zip = "28195",
                                    postfilialNumber = "547",
                                    postNumber = "960873749",
                                    Origin = new CountryType()
                                    {
                                        countryISOCode = "DE"
                                    }
                                }
                            }
                        }
                    }
                }
            });

            var t = response.ValidateShipmentResponse;

            Console.WriteLine(string.Join(" | ", t.Status.statusMessage) + string.Join(" | ", t.Status.statusText) + string.Join(" | ", t.Status.statusCode));

            //Ihr Passwort ist abgelaufen. Zur Entsperrung Ihres Benutzers verwenden Sie bitte die Funktion
            //'Passwort oder Benutzername vergessen?' im DHL Geschäftskundenportal und speichern Sie ein neues Passwort.
            //Password expired
            //112


            var tt = "";
        }
    }
}