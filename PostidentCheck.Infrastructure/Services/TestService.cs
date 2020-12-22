using KeePass.Models;
using Postident.Application.Common.Interfaces;
using Postident.Infrastructure.Common;
using Postident.Infrastructure.Services.DHL;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services
{
    public class TestService
    {
        private IDataPackReadRepository DataPackRepo { get; }
        private IInfoPackDbContext InfoPackContext { get; }

        public TestService(IDataPackReadRepository dataPackRepo, IInfoPackDbContext infoPackContext)
        {
            DataPackRepo = dataPackRepo;
            InfoPackContext = infoPackContext;
        }

        public async Task test()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://cig.dhl.de/services/production/soap/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            client.DefaultRequestHeaders.Add("SOAPAction", "\"urn:validateShipment\"");

            var byteArray = Encoding.ASCII.GetBytes(string.Join(':', "postident_validator_1", "4H4lnXBqGDS26xrLw4FytmvnzsDTXi"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var builder = new ValidationRequestXmlBuilder();

            var requestString = builder.SetUpAuthorization(new Secret()
            {
                Password = "!FuckFCB1900!",
                Username = "gartenundfreizeit"
            })
                .AddNewShipment()
                .SetUpReceiverData(new Address()
                {
                    City = "Quickborn",
                    CountryCode = "DE",
                    Street = "Packstation",
                    StreetNumber = "109",
                    PostIdent = "917741818",
                    ZipCode = "25451",
                    Name = "aaa"
                })
                .SetUpId("123456")
                .BuildShipment()
                .AddNewShipment()
                .SetUpShippingDate(DateTime.Today)
                .SetUpSenderData(new Address()
                {
                    Name = "New sender",
                    City = "Berlin",
                    CountryCode = "de",
                    Street = "Lichtenrader Str.",
                    StreetNumber = "50",
                    ZipCode = "12049"
                })
                .SetUpId("new new new")
                .SetUpReceiverData(new Address()
                {
                    City = "Alsheim",
                    CountryCode = "de",
                    Street = "Postfiliale",
                    StreetNumber = "578",
                    ZipCode = "67577",
                    PostIdent = "923805513"
                })
                .SetUpAccountNumber("62203625500104")
                .SetUpItemDimensions(1, 12, 21, 12)
                .BuildShipment()
                .Build();

            var request = new HttpRequestMessage(HttpMethod.Post, "") { Content = new StringContent(requestString) };

            var response = await client.SendAsync(request);

            var res = await response.Content.ReadAsStringAsync();

            var deser = new DhlResponseDeserializer("dhl", null);

            var drespo = await deser.Deserialize(response);

            Console.WriteLine(string.Join("\n", drespo.Responses));

            var t = "";
        }
    }
}