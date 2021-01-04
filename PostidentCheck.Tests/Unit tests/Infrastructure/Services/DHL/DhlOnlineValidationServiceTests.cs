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
        private ITestOutputHelper Output { get; }
        private AutoMocker Mocker;

        public DhlOnlineValidationServiceTests(ITestOutputHelper output)
        {
            Output = output;
            MockerSetup();
        }

        private void MockerSetup()
        {
            var settings = DhlSettingsFixture.GetProperSettings(10, 1);
            Mocker = new AutoMocker();
            Mocker.Use<IDhlSettings>(settings);
            Mocker.Use<IDefaultShipmentValues>(TestShipmentDefaultValuesFixture.Defaults());
            Mocker.Use<ILogger<DhlOnlineValidationService>>(Output.BuildLoggerFor<DhlOnlineValidationService>());
            Mocker.Use<IValidationRequestXmlBuilderFactory>(new ValidationRequestXmlBuilderFactory(TestShipmentDefaultValuesFixture.Defaults()));
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

        private HttpResponseMessage Response;

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
            var builder = new HttpRequestInterceptionBuilder()
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
            var builder = new HttpRequestInterceptionBuilder()
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
            var builder = new HttpRequestInterceptionBuilder()
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
            Assert.True(responsesText == "okokok");
        }
    }
}