using System.Linq;
using Postident.Application.Common.Validators;
using Postident.Application.DHL;
using Postident.Tests.Unit_tests.Infrastructure.Services.DHL.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Common.Validators
{
    public class DhlParcelResponseDtoValidatorTests
    {
        public DhlParcelResponseDtoValidatorTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private ITestOutputHelper Output { get; }

        [Fact]
        public void Will_validate_proper_dto()
        {
            var val = new DhlParcelResponseDtoValidator(Output.BuildLoggerFor<DhlParcelResponseDtoValidator>());
            var test = DhlResponseDtoFixture.ProperDhlResponseDtoFrom("new status", "123456").Parcels.First();

            Assert.True(val.Validate(test).IsValid);
        }

        [Fact]
        public void Will_return_false_if_checking_null()
        {
            var val = new DhlParcelResponseDtoValidator(Output.BuildLoggerFor<DhlParcelResponseDtoValidator>());

            Assert.False(val.Validate(null as DhlParcelResponseDto).IsValid);
        }

        [Fact]
        public void Will_return_false_if_PieceStatus_is_not_null()
        {
            var val = new DhlParcelResponseDtoValidator(Output.BuildLoggerFor<DhlParcelResponseDtoValidator>());
            var test = DhlResponseDtoFixture.GetNotFoundDhlResponseDtoFrom("new status", "123456").Parcels.First();

            Assert.False(val.Validate(null as DhlParcelResponseDto).IsValid);
        }
    }
}