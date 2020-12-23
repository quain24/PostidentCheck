using Postident.Application.Common.Extensions;
using Postident.Application.Common.Validators;
using Postident.Core.Entities;
using Postident.Core.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Common.Validators
{
    public class DataPackReadModelValidatorTests
    {
        public DataPackReadModelValidatorTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private ITestOutputHelper Output { get; }

        [Fact]
        public void Will_validate_proper_dto()
        {
            var val = new DataPackReadModelValidator();
            var test = new DataPackReadModel()
            {
                Carrier = Carrier.DHL,
                Street = "Norm strasse",
                City = "berlin",
                CountryCode = "DE",
                DataPackChecked = -1,
                Id = "12345",
                PostIdent = "123456789",
                ZipCode = "65888"
            };

            Assert.True(val.Validate(test).IsValid);
        }

        [Fact]
        public void Will_return_false_if_checking_null()
        {
            var val = new DataPackReadModelValidator();

            Assert.False(val.Validate(null as DataPackReadModel).IsValid);
        }

        [Fact]
        public void Will_return_false_if_ID_is_empty()
        {
            var val = new DataPackReadModelValidator();
            var test = new DataPackReadModel()
            {
                Carrier = Carrier.DHL,
                Street = "Norm strasse",
                City = "berlin",
                CountryCode = "DE",
                DataPackChecked = -1,
                Id = "",
                PostIdent = "123456789",
                ZipCode = "65888"
            };

            var result = val.Validate(test);
            Assert.False(result.IsValid);
            Output.WriteLine(result.CombinedErrors());
        }

        [Fact]
        public void Will_return_true_if_postident_is_empty()
        {
            var val = new DataPackReadModelValidator();
            var test = new DataPackReadModel()
            {
                Carrier = Carrier.DHL,
                Street = "Norm strasse",
                City = "berlin",
                CountryCode = "DE",
                DataPackChecked = -1,
                Id = "123",
                PostIdent = "",
                ZipCode = "65888"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Will_return_false_if_postident_is_not_empty_but_not_number()
        {
            var val = new DataPackReadModelValidator();
            var test = new DataPackReadModel()
            {
                Carrier = Carrier.DHL,
                Street = "Norm strasse",
                City = "berlin",
                CountryCode = "DE",
                DataPackChecked = -1,
                Id = "1",
                PostIdent = "A123456789",
                ZipCode = "65888"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.False(result.IsValid);
        }
    }
}