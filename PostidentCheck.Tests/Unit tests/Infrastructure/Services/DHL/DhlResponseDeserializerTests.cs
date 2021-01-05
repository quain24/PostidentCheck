using Postident.Infrastructure.Services.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class DhlResponseDeserializerTests
    {
        private ITestOutputHelper Output { get; }

        public DhlResponseDeserializerTests(ITestOutputHelper output)
        {
            Output = output;
            Deserializer = new DhlResponseDeserializer("Carrier", Output.BuildLoggerFor<DhlResponseDeserializer>());
        }

        public DhlResponseDeserializer Deserializer { get; }

        [Fact]
        public async Task Will_deserialize_proper_message()
        {
            var message = DhlResponseFixtures.ProperResponseSingleInfoString("123");

            var actual = await Deserializer.Deserialize(new HttpResponseMessage() { Content = new StringContent(message) });

            Assert.True(actual.MainFaultCode == "0");
            Assert.True(actual.MainFaultText == "ok");
            Assert.True(actual.Responses.Count == 1);
            Assert.True(actual.Responses[0].ErrorCode == 0);
            Assert.True(actual.Responses[0].ErrorText == "ok");
            Assert.True(actual.Responses.First().Key == "123");
        }

        [Fact]
        public async Task Will_deserialize_proper_dual_message()
        {
            var message = DhlResponseFixtures.ProperResponseDualInfoString();

            var actual = await Deserializer.Deserialize(new HttpResponseMessage() { Content = new StringContent(message) });

            Assert.True(actual.Responses.Count == 2);
        }

        public static IEnumerable<object[]> MajorXmlErrorResponses =>
            new List<object[]>
            {
                new object[] {DhlResponseFixtures.MajorXmlErrorResponse()},
                new object[] {DhlResponseFixtures.MajorXmlErrorCountry()},
                new object[] {DhlResponseFixtures.WrongLoginResponse() }
            };

        [Theory]
        [MemberData(nameof(MajorXmlErrorResponses))]
        public async Task Will_deserialize_major_error_message_faulty_xml(string response)
        {
            var actual = await Deserializer.Deserialize(new HttpResponseMessage() { Content = new StringContent(response) });

            Assert.True(actual.Responses.Count == 0);
            Output.WriteLine(actual.MainFaultCode + " || " + actual.MainFaultText);
        }

        [Fact]
        public async Task Will_deserialize_response_about_error_in_validated_object()
        {
            var message = DhlResponseFixtures.MinorErrorBadWeightAndCityResponse("123");

            var actual = await Deserializer.Deserialize(new HttpResponseMessage() { Content = new StringContent(message) });

            Assert.True(actual.MainFaultCode == "1101");
            Assert.True(actual.MainFaultText == "Hard validation error occured.");
            Assert.True(actual.Responses.Count == 1);
            Assert.True(actual.Responses[0].ErrorCode == 1101);
            Assert.True(actual.Responses[0].ErrorText == "Hard validation error occured.");
            Assert.True(actual.Responses.First().StatusMessages.Count == 2);
            Assert.True(actual.Responses.First().Key == "123");

            Output.WriteLine(actual.Responses.First());
        }

        [Fact]
        public async Task Will_throw_null_reference_if_given_null_instead_of_message()
        {
            HttpResponseMessage message = null;

            await Assert.ThrowsAsync<NullReferenceException>(async () => await Deserializer.Deserialize(message));
        }

        [Fact]
        public async Task Will_return_null_if_given_damaged_xml_to_deserialize()
        {
            var message = DhlResponseFixtures.ProperResponseSingleInfoString().Remove(10, 25);

            var actual = await Deserializer.Deserialize(new HttpResponseMessage() { Content = new StringContent(message) });

            Assert.Null(actual);
        }
    }
}