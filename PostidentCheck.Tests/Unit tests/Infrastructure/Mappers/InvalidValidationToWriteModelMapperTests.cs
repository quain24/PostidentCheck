using System;
using FluentValidation.Results;
using Postident.Infrastructure.Mappers;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Mappers
{
    public class InvalidValidationToWriteModelMapperTests
    {
        private ITestOutputHelper Output { get; }

        public InvalidValidationToWriteModelMapperTests(ITestOutputHelper output)
        {
            Output = output;
            Mapper = new InvalidValidationToWriteModelMapper();
        }

        public InvalidValidationToWriteModelMapper Mapper { get; }

        [Fact]
        public void Will_map_correct_data()
        {
            var data = new ValidationResult()
            {
                Errors = {new ValidationFailure("Any prop", "test error message")}
            };

            var actual = Mapper.MapInvalidResult(data, "1");

            Assert.NotNull(actual);
            Assert.True(actual.Id == "1");
            Output.WriteLine(actual.Message);
        }

        [Fact]
        public void Will_map_correct_data_without_id()
        {
            var data = new ValidationResult()
            {
                Errors = { new ValidationFailure("Any prop", "test error message") }
            };

            var actual = Mapper.MapInvalidResult(data, null);

            Assert.NotNull(actual);
            Assert.True(actual.Id == string.Empty);
            Output.WriteLine(actual.Message);
        }

        [Fact]
        public void Throws_arg_out_of_range_exc_when_trying_to_map_valid_result()
        {
            var data = new ValidationResult();

            Assert.Throws<ArgumentOutOfRangeException>(() => Mapper.MapInvalidResult(data, "1"));
        }

        [Fact]
        public void Throws_null_exc_when_trying_to_map_null()
        {
            Assert.Throws<ArgumentNullException>(() => Mapper.MapInvalidResult(null as ValidationResult, "1"));
        }
    }
}