using Postident.Application.Common.Validators;
using Postident.Application.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Common.Validators
{
    public class DhlResponseDtoValidatorTests
    {
        private ITestOutputHelper Output { get; }

        public DhlResponseDtoValidatorTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Will_return_false_when_checking_null()
        {
            var val = new DhlResponseDtoValidator(Output.BuildLoggerFor<DhlResponseDtoValidator>());

            Assert.False(val.Validate(null as DhlResponseDto).IsValid);
        }

        [Fact]
        public void Will_validate_proper_dto()
        {
            var test = DhlResponseDtoFixture.ProperDhlResponseDtoFrom("status", "123456");
            var val = new DhlResponseDtoValidator(Output.BuildLoggerFor<DhlResponseDtoValidator>());

            Assert.True(val.Validate(test).IsValid);
        }

        [Fact]
        public void Will_return_false_if_ErrorCode_is_not_0()
        {
            var val = new DhlResponseDtoValidator(Output.BuildLoggerFor<DhlResponseDtoValidator>());
            var test = new DhlResponseDto() { ErrorCode = 100 };

            Assert.False(val.Validate(test).IsValid);
        }
    }
}