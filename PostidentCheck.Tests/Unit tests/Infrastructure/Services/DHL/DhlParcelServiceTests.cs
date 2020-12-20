using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.ExpressionBuilders.Logging;
using Postident.Core.Entities;
using Postident.Infrastructure.Services.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using Xunit;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class DhlParcelServiceTests : IClassFixture<HttpClientFactoryFixtures>
    {
        private readonly Mock<ILogger<DhlPostidentService>> _loggerMock = new();
        private readonly Mock<IValidator<DataPack>> _validatorMock = new();

        private DhlPostidentService DhlService { get; set; }

        [Fact]
        public async Task Given_proper_parcel_objects_and_responses_should_return_dtos()
        {
            _validatorMock.Setup((v) => v.Validate(It.IsAny<DataPack>()).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();
            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlOkCodeFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, 1));

            var actual = await DhlService.GetParcelInfoFromProviderAsync(ParcelFixture.GetProperParcelCollection("OK"), CancellationToken.None);
            actual = actual.OrderBy(p => double.Parse(p.Parcels.First().SearchedPieceCode));

            Assert.Equal(ParcelFixture.GetProperParcelCollection("").Count(), actual.Count());
            Assert.Collection(actual, dto => Assert.Equal(dto.Parcels.First().SearchedPieceCode, ParcelNumbersFixture.ProperParcelNumbers[0]),
                dto => Assert.Equal(dto.Parcels.First().SearchedPieceCode, ParcelNumbersFixture.ProperParcelNumbers[1]),
                dto => Assert.Equal(dto.Parcels.First().SearchedPieceCode, ParcelNumbersFixture.ProperParcelNumbers[2]),
                dto => Assert.Equal(dto.Parcels.First().SearchedPieceCode, ParcelNumbersFixture.ProperParcelNumbers[3]));
        }

        [Fact]
        public async Task Given_null_will_return_empty_collection()
        {
            _validatorMock.Setup((v) => v.Validate(It.IsAny<DataPack>()).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();
            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlOkCodeFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, 1));

            var actual = await DhlService.GetParcelInfoFromProviderAsync(null, CancellationToken.None);

            Assert.True(!actual.Any());
        }

        [Fact]
        public async Task Given_empty_collection_will_return_empty_collection()
        {
            _validatorMock.Setup((v) => v.Validate(It.IsAny<DataPack>()).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();
            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlOkCodeFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, 1));

            var actual = await DhlService.GetParcelInfoFromProviderAsync(new List<DataPack>(), CancellationToken.None);

            Assert.True(!actual.Any());
        }

        [Fact]
        public async Task Given_all_bad_xml_in_response_will_return_empty_collection()
        {
            _validatorMock.Setup((v) => v.Validate(It.IsAny<DataPack>()).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();

            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlBadRequestFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, 1));

            var actual = await DhlService.GetParcelInfoFromProviderAsync(ParcelFixture.GetProperParcelCollection(""), CancellationToken.None);

            Assert.True(!actual.Any());
        }

        [Fact]
        public async Task Given_all_bad_xml_format_in_response_will_return_empty_collection()
        {
            _validatorMock.Setup((v) => v.Validate(It.IsAny<DataPack>()).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();

            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlBadRequestFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, 1));

            var actual = await DhlService.GetParcelInfoFromProviderAsync(ParcelFixture.GetProperParcelCollection(""), CancellationToken.None);

            Assert.True(!actual.Any());
        }

        [Fact]
        public async Task If_all_of_responses_will_come_as_not_found_error_in_deserialized_xml_will_return_empty_collection()
        {
            int numbersPerQuery = 2;
            _validatorMock.Setup((v) => v.Validate(It.IsAny<DataPack>()).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();
            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlTrackingNotFoundFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, numbersPerQuery));

            var actual = await DhlService.GetParcelInfoFromProviderAsync(ParcelFixture.GetProperParcelCollection(""), CancellationToken.None);

            Assert.True(actual.Count() == (ParcelFixture.GetProperParcelCollection("").Count()) / numbersPerQuery);
        }

        [Fact]
        public async Task Given_proper_parcels_and_null_parcels_in_one_go_will_process_non_nulls_and_log_errors()
        {
            int numbersPerQuery = 2;
            _validatorMock.Setup((v) => v.Validate(It.Is<DataPack>((f) => f == null)).IsValid).Returns(false);
            _validatorMock.Setup((v) => v.Validate(It.Is<DataPack>((f) => f != null)).IsValid).Returns(true);
            var logMock = new Mock<ILogger<DhlResponseDeserializer>>();
            DhlService = new DhlPostidentService(HttpClientFactoryFixtures.DhlOkCodeFactory, new DhlResponseDeserializer("", logMock.Object), _validatorMock.Object, _loggerMock.Object, DhlSettingsFixture.GetProperSettings(10, numbersPerQuery));

            var data = ParcelFixture.GetProperParcelCollection("ok").ToList();
            data[0] = null;
            data[2] = null;

            var actual = await DhlService.GetParcelInfoFromProviderAsync(data, CancellationToken.None);

            _loggerMock.Verify(Log.With.LogLevel(LogLevel.Error), Times.Exactly(2));
            Assert.True(actual.Sum(p => p.Parcels.Count) == (ParcelFixture.GetProperParcelCollection("").Count()) - 2);
        }
    }
}