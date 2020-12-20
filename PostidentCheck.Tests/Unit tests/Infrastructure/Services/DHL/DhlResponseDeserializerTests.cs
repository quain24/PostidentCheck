using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.ExpressionBuilders.Logging;
using Postident.Infrastructure.Services.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using Xunit;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class DhlResponseDeserializerTests
    {
        public DhlResponseDeserializerTests()
        {
            Deserializer = new DhlResponseDeserializer("Carrier", LogMock.Object);
        }

        private Mock<ILogger<DhlResponseDeserializer>> LogMock = new Mock<ILogger<DhlResponseDeserializer>>();

        private DhlResponseDeserializer Deserializer { get; }

        [Fact]
        public async Task Will_deserialize_proper_message()
        {
            var message = DhlResponseMessagesFixtures.GetProperHttpResponseMessageFrom("123456789");

            var actual = await Deserializer.Deserialize(message);

            Assert.True(actual.Parcels.Count == 1);
            Assert.Equal("123456789", actual.Parcels.First().SearchedPieceCode);
        }

        [Fact]
        public async Task Will_deserialize_error_message()
        {
            var message = DhlResponseMessagesFixtures.GetNotFoundXMLResponse("123456789");

            var actual = await Deserializer.Deserialize(message);

            Assert.True(actual.Parcels.Count == 1);
            Assert.Equal("123456789", actual.Parcels.First().SearchedPieceCode);
        }

        [Fact]
        public async Task Will_return_null_and_log_error_if_given_incorrect_response()
        {
            var message = DhlResponseMessagesFixtures.GetBadRequestCodeResponse();

            var actual = await Deserializer.Deserialize(message);

            LogMock.Verify(Log.With.LogLevel(LogLevel.Error), Times.Exactly(1));
            Assert.Null(actual);
        }

        [Fact]
        public async Task Will_throw_null_reference_if_given_null_instead_of_message()
        {
            HttpResponseMessage message = null;

            await Assert.ThrowsAsync<NullReferenceException>(async () => await Deserializer.Deserialize(message));
        }
    }
}