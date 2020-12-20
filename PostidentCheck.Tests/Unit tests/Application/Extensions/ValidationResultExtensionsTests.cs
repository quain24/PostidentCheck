using System;
using FluentValidation.Results;
using Postident.Application.Common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Application.Extensions
{
    public class ValidationResultExtensionsTests
    {
        private ITestOutputHelper Output { get; }

        public ValidationResultExtensionsTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Will_throw_nullexception_when_trying_to_combine_errors_from_null()
        {
            ValidationResult result = null;
            Assert.Throws<ArgumentNullException>(() => result.CombinedErrors());
        }

        [Fact]
        public void Will_join_errors_if_there_are_any()
        {
            var result = new ValidationResult()
            { Errors = { new ValidationFailure("prop1", "error1"), new ValidationFailure("prop2", "error2") } };

            var actual = result.CombinedErrors();

            Assert.Equal("prop1 - error1 | prop2 - error2", actual);
        }

        [Fact]
        public void Will_return_empty_string_if_no_error_messages_to_join()
        {
            var result = new ValidationResult();

            var actual = result.CombinedErrors();

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void Will_return_single_error_message_if_only_one_error_exists()
        {
            var result = new ValidationResult()
            { Errors = { new ValidationFailure("prop1", "error1") } };

            var actual = result.CombinedErrors();

            Assert.Equal("prop1 - error1", actual);
        }
    }
}