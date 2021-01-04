using Postident.Application.Common.Extensions;
using Postident.Application.Common.Models;
using Postident.Application.Common.Validators;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Common.Validators
{
    public class AddressValidatorTests
    {
        private ITestOutputHelper Output { get; }

        public AddressValidatorTests(ITestOutputHelper output)
        {
            Output = output;
            Validator = new AddressValidator();
        }

        public AddressValidator Validator { get; }

        [Fact]
        public void Will_validate_proper_data_as_valid()
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "12345",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "11235"
            };

            var actual = Validator.Validate(data);

            Assert.True(actual.IsValid);
        }

        [Fact]
        public void Will_validate_proper_data_as_valid_if_postident_empty()
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "11235"
            };

            var actual = Validator.Validate(data);

            Assert.True(actual.IsValid);
        }

        [Fact]
        public void Will_validate_proper_data_as_valid_if_zipcode_empty()
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "1234567",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = ""
            };

            var actual = Validator.Validate(data);

            Assert.True(actual.IsValid);
        }

        [Fact]
        public void Will_validate_proper_data_as_valid_if_name_empty()
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "",
                PostIdent = "123456",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "11235"
            };

            var actual = Validator.Validate(data);

            Assert.True(actual.IsValid);
        }

        [Fact]
        public void Invalid_if_postident_is_not_a_number()
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "AB123456",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "01235"
            };

            var actual = Validator.Validate(data);

            Assert.False(actual.IsValid);
            Output.WriteLine(actual.CombinedErrors());
        }

        [Fact]
        public void Invalid_if_zipcode_is_not_a_number()
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "123456",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "A1235"
            };

            var actual = Validator.Validate(data);

            Assert.False(actual.IsValid);
            Output.WriteLine(actual.CombinedErrors());
        }

        [Fact]
        public void Invalid_if_city_is_missing()
        {
            var data = new Address()
            {
                City = "",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "123456",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "11235"
            };

            var actual = Validator.Validate(data);

            Assert.False(actual.IsValid);
            Output.WriteLine(actual.CombinedErrors());
        }

        [Theory]
        [InlineData("0012")]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("1234566")]
        public void Invalid_if_zipcode_is_not_correct_number(string zip)
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = "pl",
                Name = "Adam Nowak",
                PostIdent = "123456",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = zip
            };

            var actual = Validator.Validate(data);

            Assert.False(actual.IsValid);
            Output.WriteLine(actual.CombinedErrors());
        }

        [Theory]
        [InlineData("")]
        [InlineData("_d")]
        [InlineData("D")]
        [InlineData("deee")]
        public void Invalid_if_country_code_is_not_correct(string countryCode)
        {
            var data = new Address()
            {
                City = "Poznań",
                CountryCode = countryCode,
                Name = "Adam Nowak",
                PostIdent = "123456",
                Street = "Ułańska",
                StreetNumber = "32A",
                ZipCode = "12345"
            };

            var actual = Validator.Validate(data);

            Assert.False(actual.IsValid);
            Output.WriteLine(actual.CombinedErrors());
        }
    }
}