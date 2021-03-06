using Postident.Application.Common.Extensions;
using Postident.Application.Common.Models;
using Postident.Application.Common.Validators;
using Postident.Core.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Common.Validators
{
    public class DataPackValidatorTests
    {
        public DataPackValidatorTests(ITestOutputHelper output)
        {
            Output = output;
        }

        private ITestOutputHelper Output { get; }

        [Fact]
        public void Will_validate_proper_dto()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "123456789",
                    ZipCode = "65888"
                },
                Email = "proper.email.ger@wp.de",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            Assert.True(val.Validate(test).IsValid);
        }

        [Fact]
        public void Will_be_valid_if_email_Is_empty()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "123456789",
                    ZipCode = "65888"
                },
                Email = "",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            Assert.True(val.Validate(test).IsValid);
        }

        [Fact]
        public void Will_be_valid_if_email_Is_null()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "123456789",
                    ZipCode = "65888"
                },
                Email = null,
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            Assert.True(val.Validate(test).IsValid);
        }

        [Theory]
        [InlineData("  email@wp.pl")]
        [InlineData("email..@wp.pl")]
        [InlineData("email.@wp.pl")]
        [InlineData("email.new.@wp.pl")]
        [InlineData("email.new.123@ wp.pl")]
        [InlineData("email.new.123@wp.berlinnn")]
        [InlineData("email.new.123@pllll")]
        [InlineData("email.new<.123@pllll.cop")]
        public void Will_return_false_if_email_Is_invalid(string email)
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "123456789",
                    ZipCode = "65888"
                },
                Email = email,
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            Assert.False(val.Validate(test).IsValid);
        }

        [Fact]
        public void Will_return_false_if_checking_null()
        {
            var val = new DataPackValidator(new AddressValidator());

            Assert.False(val.Validate(null as DataPack).IsValid);
        }

        [Fact]
        public void Will_return_false_if_ID_is_empty()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "123456789",
                    ZipCode = "65888"
                },
                Email = "test@test.com",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = ""
            };

            var result = val.Validate(test);
            Assert.False(result.IsValid);
            Output.WriteLine(result.CombinedErrors());
        }

        [Fact]
        public void Will_return_true_if_postident_is_empty()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "",
                    ZipCode = "65888"
                },
                Email = "test@test.com",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Will_return_false_if_postident_is_not_empty_but_not_number()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "AA123456789",
                    ZipCode = "65888"
                },
                Email = "test@test.com",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_return_false_if_carrier_is_not_in_enum()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = (Carrier)33,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "AA123456789",
                    ZipCode = "65888"
                },
                Email = "test@test.com",
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Id = "12345"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_return_false_if_Data_pack_checked_is_not_in_enum()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                Address = new Address()
                {
                    Street = "Norm strasse",
                    City = "berlin",
                    CountryCode = "DE",
                    PostIdent = "AA123456789",
                    ZipCode = "65888"
                },
                Email = "test@test.com",
                DataPackChecked = (InfoPackCheckStatus)33,
                Id = "12345"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Will_return_false_if_address_is_missing()
        {
            var val = new DataPackValidator(new AddressValidator());
            var test = new DataPack()
            {
                Carrier = Carrier.DHL,
                DataPackChecked = InfoPackCheckStatus.Unchecked,
                Email = "test@test.com",
                Id = "12345"
            };

            var result = val.Validate(test);
            Output.WriteLine(result.CombinedErrors());
            Assert.False(result.IsValid);
        }
    }
}