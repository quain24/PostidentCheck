using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures
{
    public class HttpClientFactoryFixtures
    {
        static HttpClientFactoryFixtures()
        {
            DhlOkCodeFactory = CreateDhlOkCode();
            DhlTrackingNotFoundFactory = CreateDhlNotFoundFactory();
            DhlBadRequestFactory = CreateDhlBadRequestFactory();
        }

        public static IHttpClientFactory DhlOkCodeFactory { get; }
        public static IHttpClientFactory DhlTrackingNotFoundFactory { get; }
        public static IHttpClientFactory DhlBadRequestFactory { get; }

        private static IHttpClientFactory CreateDhlOkCode()
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    var response = DhlResponseMessagesFixtures.GetProperResponseFromRequest(request).GetAwaiter().GetResult();
                    return response;
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            client.BaseAddress = new Uri("https://www.noname.com/");

            return httpClientFactory.Object;
        }

        private static IHttpClientFactory CreateDhlNotFoundFactory()
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    var response = DhlResponseMessagesFixtures.GetNotFoundResponsesFromRequest(request).GetAwaiter()
                        .GetResult();
                    return response;
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            client.BaseAddress = new Uri("https://www.noname.com/");

            return httpClientFactory.Object;
        }

        private static IHttpClientFactory CreateDhlBadRequestFactory()
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    var response = DhlResponseMessagesFixtures.GetBadRequestCodeResponse();
                    return response;
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            client.BaseAddress = new Uri("https://www.noname.com/");

            return httpClientFactory.Object;
        }
    }
}