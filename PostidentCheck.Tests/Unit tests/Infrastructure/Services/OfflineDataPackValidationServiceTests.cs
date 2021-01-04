using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.AutoMock;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using Postident.Core.Enums;
using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Services
{
    public class OfflineDataPackValidationServiceTests
    {
        private ITestOutputHelper Output { get; }

        public OfflineDataPackValidationServiceTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Will_return_same_collection_when_all_data_packs_are_valid()
        {
            var mock = new Mock<IValidator<DataPack>>();
            mock.Setup(m => m.Validate(It.IsAny<DataPack>())).Returns(new ValidationResult());
            var mapMock = new AutoMocker();
            var mapper = mapMock.GetMock<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>>();
            var test = new OfflineDataPackValidationService(mock.Object, mapper.Object,
                Output.BuildLoggerFor<OfflineDataPackValidationService>());

            var data = new List<DataPack>()
            {
                new()
                {
                    Address = new Address(),
                    Carrier = Carrier.DHL,
                    DataPackChecked = InfoPackCheckStatus.Valid,
                    Id = "1"
                },
                new()
                {
                    Address = new Address(),
                    Carrier = Carrier.DHL,
                    DataPackChecked = InfoPackCheckStatus.Valid,
                    Id = "2"
                }
            };

            var result = test.FilterOutInvalidDataPacksFrom(data, out var invalid);

            Assert.True(result.Count() == 2);
            Assert.Collection(result, d => Assert.Contains("1", d.Id),
                                      d => Assert.Contains("2", d.Id));
            Assert.Empty(invalid);
        }

        [Fact]
        public void Will_return_good_model_and_out_invalid_data_when_given_mixed_data()
        {
            var mock = new Mock<IValidator<DataPack>>();
            mock.Setup(m => m.Validate(It.Is<DataPack>(d => d.Id == "1"))).Returns(new ValidationResult());
            mock.Setup(m => m.Validate(It.Is<DataPack>(d => d.Id == "2")))
                .Returns(new ValidationResult(new List<ValidationFailure> { new("Id", "Test err message") { ErrorCode = "2", ErrorMessage = "Test Error Message" } }));

            var mocker = new AutoMocker();
            var mapper = mocker.GetMock<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>>();
            mapper.Setup(m => m.MapInvalidResult(It.IsAny<ValidationResult>(), It.Is<string>(s => s == "2"))).Returns(
                new InfoPackWriteModel
                {
                    CheckStatus = InfoPackCheckStatus.Invalid,
                    Id = "2",
                    Message = "Error message"
                });

            var test = new OfflineDataPackValidationService(mock.Object, mapper.Object,
                Output.BuildLoggerFor<OfflineDataPackValidationService>());

            var data = new List<DataPack>()
            {
                new()
                {
                    Address = new Address(),
                    Carrier = Carrier.DHL,
                    DataPackChecked = InfoPackCheckStatus.Valid,
                    Id = "1"
                },
                new()
                {
                    Address = new Address(),
                    Carrier = Carrier.DHL,
                    DataPackChecked = InfoPackCheckStatus.Valid,
                    Id = "2"
                }
            };

            var result = test.FilterOutInvalidDataPacksFrom(data, out var invalid);

            Assert.True(result.Count() == 1);
            Assert.Collection(result, d => Assert.Contains("1", d.Id));
            Assert.True(invalid.Count() == 1);
            Assert.Collection(invalid, d => Assert.Contains("2", d.Id));
        }

        [Fact]
        public void Given_no_data_packs_will_return_empty_collections()
        {
            var mocker = new AutoMocker();
            var validator = mocker.GetMock<IValidator<DataPack>>();
            validator.Setup(v => v.Validate(It.IsAny<DataPack>())).Returns(new ValidationResult());

            var test = new OfflineDataPackValidationService(validator.Object,
                mocker.GetMock<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>>().Object,
                Output.BuildLoggerFor<OfflineDataPackValidationService>());

            var result = test.FilterOutInvalidDataPacksFrom(Array.Empty<DataPack>(), out var invalids);

            Assert.Empty(result);
            Assert.Empty(invalids);
        }

        [Fact]
        public void Given_packs_that_have_no_id_will_return_empty_collections()
        {
            var mocker = new AutoMocker();
            var validator = mocker.GetMock<IValidator<DataPack>>();
            validator.Setup(v => v.Validate(It.IsAny<DataPack>()))
                .Returns(new ValidationResult(new List<ValidationFailure> { new("Id", "Missing id") { ErrorCode = "id_missing" } }));

            var test = new OfflineDataPackValidationService(validator.Object,
                mocker.GetMock<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>>().Object,
                Output.BuildLoggerFor<OfflineDataPackValidationService>());

            var result = test.FilterOutInvalidDataPacksFrom(new List<DataPack>(){new DataPack(), new DataPack(), new DataPack()}, out var invalids);

            Assert.Empty(result);
            Assert.Empty(invalids);
        }

        [Fact]
        public void Given_null_will_throw_arg_null_exc()
        {
            var mocker = new AutoMocker();
            var validator = mocker.GetMock<IValidator<DataPack>>();
            validator.Setup(v => v.Validate(It.IsAny<DataPack>())).Returns(new ValidationResult());

            var test = new OfflineDataPackValidationService(validator.Object,
                mocker.GetMock<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>>().Object,
                Output.BuildLoggerFor<OfflineDataPackValidationService>());

            Assert.Throws<ArgumentNullException>(() => test.FilterOutInvalidDataPacksFrom(null, out var invalids));
        }
    }
}