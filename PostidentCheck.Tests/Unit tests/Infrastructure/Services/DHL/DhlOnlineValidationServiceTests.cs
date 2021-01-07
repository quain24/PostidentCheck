using JustEat.HttpClientInterception;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Application.DHL;
using Postident.Core.Enums;
using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Interfaces.DHL;
using Postident.Infrastructure.Services.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class DhlOnlineValidationServiceTests
    {
        public DhlOnlineValidationServiceTests(ITestOutputHelper output)
        {
            Output = output;
            MockerSetup();
        }

        private ITestOutputHelper Output { get; }
        private AutoMocker Mocker { get; set; }
        private HttpResponseMessage Response { get; set; }

        private void MockerSetup()
        {
            Mocker = new AutoMocker();
            var builder = Mocker.GetMock<IValidationRequestXmlBuilder>().Object;

            Mocker.Use<IDhlSettings>(DhlSettingsFixture.GetProperSettings(10, 1));
            Mocker.Use<IDefaultShipmentValues>(TestShipmentDefaultValuesFixture.Defaults());
            Mocker.Use<ILogger<DhlOnlineValidationService>>(Output.BuildLoggerFor<DhlOnlineValidationService>());
            Mocker.GetMock<ISingleShipmentBuilder>().Setup(b => b.BuildShipment()).Returns(builder);
            Mocker.GetMock<IValidationRequestXmlBuilderFactory>().Setup(b => b.CreateInstance()).Returns(builder);
            Mocker.GetMock<IValidationRequestXmlBuilder>().Setup(b => b.Build()).Returns("xml validation request string");
            Mocker.GetMock<IValidationRequestXmlBuilder>().Setup(b => b.SetUpAuthorization(It.IsAny<string>(), It.IsAny<string>())).Returns(builder);
            Mocker.GetMock<IValidationRequestXmlBuilder>().Setup(b => b.AddNewShipment(It.IsAny<string>(), It.IsAny<Address>())).Returns(Mocker.GetMock<ISingleShipmentBuilder>().Object);
            Mocker.GetMock<ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>>()
                .Setup(s => s.Deserialize(It.IsAny<HttpResponseMessage>()))
                .Callback<HttpResponseMessage>(r => Response = r)
                .Returns(Task.FromResult(new DhlMainResponseDto()
                {
                    MainFaultCode = "0",
                    MainFaultText = "None",
                    Responses = ImmutableList.Create<DhlResponseDto>
                    (
                        new DhlResponseDto()
                        {
                            ErrorCode = 0,
                            ErrorText = "No error",
                            Key = "123",
                            StatusMessages = ImmutableHashSet.Create("All is well")
                        }
                    )
                }));
        }

        private void SetupClient(HttpClientInterceptorOptions options)
        {
            Mocker.GetMock<IHttpClientFactory>()
                .Setup(f => f
                    .CreateClient(It.IsAny<string>()))
                .Returns(() => options.CreateHttpClient(DhlSettingsFixture.GetProperSettings(1, 1).BaseAddress));
        }

        [Fact]
        public async Task Will_handle_proper_response()
        {
            var options = new HttpClientInterceptorOptions().ThrowsOnMissingRegistration();
            new HttpRequestInterceptionBuilder()
                .Requests()
                .ForAnyHost()
                .ForHttps()
                .ForPost()
                .Responds()
                .WithStatus(HttpStatusCode.OK)
                .WithContent("test")
                .RegisterWith(options);

            SetupClient(options);
            var service = Mocker.CreateInstance<DhlOnlineValidationService>();

            var result = await service.GetResponsesFromApiAsync(new List<DataPack>()
            {
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "123"
                }
            }, CancellationToken.None);

            Assert.True(result.Count() == 1);
            Assert.True(result.First().Responses.Count == 1);
            Assert.True(Response.Content.ReadAsStringAsync().Result == "test");
        }

        [Fact]
        public async Task Will_handle_deserializable_error_response()
        {
            var options = new HttpClientInterceptorOptions().ThrowsOnMissingRegistration();
            new HttpRequestInterceptionBuilder()
                .Requests()
                .ForAnyHost()
                .ForHttps()
                .ForPost()
                .Responds()
                .WithStatus(HttpStatusCode.InternalServerError)
                .WithContent("500 err")
                .RegisterWith(options);

            SetupClient(options);
            var service = Mocker.CreateInstance<DhlOnlineValidationService>();

            var result = await service.GetResponsesFromApiAsync(new List<DataPack>()
            {
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "123"
                }
            }, CancellationToken.None);

            Assert.True(result.Count() == 1);
            Assert.True(result.First().Responses.Count == 1);
            Assert.True(Response.Content.ReadAsStringAsync().Result == "500 err");
        }

        [Fact]
        public async Task Will_handle_multiple_data_packs()
        {
            var options = new HttpClientInterceptorOptions().ThrowsOnMissingRegistration();
            new HttpRequestInterceptionBuilder()
                .Requests()
                .ForAnyHost()
                .ForHttps()
                .ForPost()
                .Responds()
                .WithStatus(HttpStatusCode.OK)
                .WithContent("ok")
                .RegisterWith(options);

            SetupClient(options);

            var responsesText = "";
            Mocker.GetMock<ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>>()
                .Setup(s => s.Deserialize(It.IsAny<HttpResponseMessage>()))
                .Callback<HttpResponseMessage>(r => responsesText += r.Content.ReadAsStringAsync().Result)
                .Returns(Task.FromResult(new DhlMainResponseDto()
                {
                    MainFaultCode = "0",
                    MainFaultText = "None",
                    Responses = ImmutableList.Create<DhlResponseDto>
                    (
                        new DhlResponseDto()
                        {
                            ErrorCode = 0,
                            ErrorText = "No error",
                            Key = "123",
                            StatusMessages = ImmutableHashSet.Create("All is well")
                        }
                    )
                }));

            var service = Mocker.CreateInstance<DhlOnlineValidationService>();

            var result = await service.GetResponsesFromApiAsync(new List<DataPack>()
            {
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "123"
                },new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "456"
                },
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "789"
                }
            }, CancellationToken.None);

            Assert.True(result.Count() == 3);
            Assert.True(result.First().Responses.Count == 1);
            // 4 ok because of validation request
            Assert.True(responsesText == "okokokok");
        }

        [Fact]
        public async Task Will_return_empty_when_xml_login_data_is_wrong()
        {
            var options = new HttpClientInterceptorOptions().ThrowsOnMissingRegistration();
            new HttpRequestInterceptionBuilder()
                .Requests()
                .ForAnyHost()
                .ForHttps()
                .ForPost()
                .Responds()
                .WithStatus(HttpStatusCode.OK)
                .WithContent("ok")
                .RegisterWith(options);

            SetupClient(options);

            var responsesText = "";
            Mocker.GetMock<ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>>()
                .Setup(s => s.Deserialize(It.IsAny<HttpResponseMessage>()))
                .Callback<HttpResponseMessage>(r => responsesText += r.Content.ReadAsStringAsync().Result)
                .Returns(Task.FromResult(new DhlMainResponseDto()
                {
                    MainFaultCode = "118",
                    MainFaultText = "Invalid GKP username and/or password.",
                    Responses = ImmutableList<DhlResponseDto>.Empty
                }));

            var service = Mocker.CreateInstance<DhlOnlineValidationService>();

            var result = await service.GetResponsesFromApiAsync(new List<DataPack>()
            {
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "123"
                },new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "456"
                },
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "789"
                }
            }, CancellationToken.None);

            Assert.Empty(result);
            Assert.True(responsesText == "ok");
        }

        [Fact]
        public async Task Will_return_empty_when_login_data_is_wrong()
        {
            var options = new HttpClientInterceptorOptions().ThrowsOnMissingRegistration();
            new HttpRequestInterceptionBuilder()
                .Requests()
                .ForAnyHost()
                .ForHttps()
                .ForPost()
                .Responds()
                .WithStatus(HttpStatusCode.Unauthorized)
                .WithContent("ok")
                .RegisterWith(options);

            SetupClient(options);

            var responsesText = "";
            Mocker.GetMock<ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>>()
                .Setup(s => s.Deserialize(It.IsAny<HttpResponseMessage>()))
                .Callback<HttpResponseMessage>(r => responsesText += r.Content.ReadAsStringAsync().Result)
                .Returns(Task.FromResult(new DhlMainResponseDto()
                {
                    MainFaultCode = "0",
                    MainFaultText = "Correct response",
                    Responses = ImmutableList.Create<DhlResponseDto>
                    (
                        new DhlResponseDto()
                        {
                            ErrorCode = 0,
                            ErrorText = "No error",
                            Key = "123",
                            StatusMessages = ImmutableHashSet.Create("All is well")
                        }
                    )
                }));

            var service = Mocker.CreateInstance<DhlOnlineValidationService>();

            var result = await service.GetResponsesFromApiAsync(new List<DataPack>()
            {
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "123"
                },new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "456"
                },
                new DataPack()
                {
                    Address = new Address(), Carrier = Carrier.DHL, DataPackChecked = InfoPackCheckStatus.Unchecked,
                    Id = "789"
                }
            }, CancellationToken.None);

            Assert.Empty(result);
            Assert.True(responsesText?.Length == 0);
        }
    }
}