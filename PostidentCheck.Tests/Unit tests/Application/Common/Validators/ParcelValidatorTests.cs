using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Postident.Application.Common.Validators;
using Postident.Core.Entities;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Common.Validators
{
    public class ParcelValidatorTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IValidator<DataPackReadModel> _validator = new ParcelValidator();

        public ParcelValidatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Will_validate_as_good_proper_parcel()
        {
            var parcel = ParcelFixture.GetProperParcelWithTrackingNumber("123", "good");

            var result = _validator.Validate(parcel);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Will_not_validate_if_checking_null()
        {
            var result = _validator.Validate(null as DataPackReadModel);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_validate_as_good_proper_parcel_with_no_status()
        {
            var parcel = ParcelFixture.GetProperParcelWithTrackingNumber("123", string.Empty);

            var result = _validator.Validate(parcel);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Will_not_validate_if_given_parcel_does_not_have_a_tracking_number()
        {
            var parcel = ParcelFixture.GetProperParcelWithTrackingNumber(string.Empty, "good");

            var result = _validator.Validate(parcel);

            PushErrorsToOutput(result);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_not_validate_if_given_parcel_does_not_have_a_valid_carrier()
        {
            var parcel = ParcelFixture.GetProperParcelWithTrackingNumber("123456789", "good");
            parcel.CarrierId = "abc";

            var result = _validator.Validate(parcel);

            PushErrorsToOutput(result);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_not_validate_if_given_parcel_does_not_have_a_carrier()
        {
            var parcel = ParcelFixture.GetProperParcelWithTrackingNumber("123456789", "good");
            parcel.CarrierId = string.Empty;

            var result = _validator.Validate(parcel);

            PushErrorsToOutput(result);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_not_validate_if_given_parcel_does_not_have_a_carrier_and_tracking_number()
        {
            var parcel = ParcelFixture.GetProperParcelWithTrackingNumber(string.Empty, "good");
            parcel.CarrierId = string.Empty;

            var result = _validator.Validate(parcel);

            PushErrorsToOutput(result);
            Assert.False(result.IsValid);
        }

        private void PushErrorsToOutput(ValidationResult result)
        {
            var message = result.Errors.Select(e => e?.ErrorCode + " - " + e?.ErrorMessage);
            _output.WriteLine(string.Join(" | ", message));
        }
    }
}