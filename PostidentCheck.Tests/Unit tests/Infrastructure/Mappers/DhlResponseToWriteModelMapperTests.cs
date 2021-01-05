using Postident.Application.DHL;
using Postident.Core.Enums;
using Postident.Infrastructure.Mappers;
using System;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Postident.Tests.Unit_tests.Infrastructure.Mappers
{
    public class DhlResponseToWriteModelMapperTests
    {
        private ITestOutputHelper Output { get; }

        public DhlResponseToWriteModelMapperTests(ITestOutputHelper output)
        {
            Output = output;
            Mapper = new DhlResponseToWriteModelMapper();
        }

        public DhlResponseToWriteModelMapper Mapper { get; }

        [Fact]
        public void Will_return_empty_collection_if_cannot_map_anything()
        {
            var data = new DhlMainResponseDto()
            {
                MainFaultCode = "0",
                MainFaultText = "ok",
                Responses = ImmutableList<DhlResponseDto>.Empty
            };

            var actual = Mapper.Map(data);

            Assert.Empty(actual);
        }

        [Fact]
        public void Will_map_correct_data()
        {
            var data = new DhlMainResponseDto()
            {
                MainFaultCode = "0",
                MainFaultText = "ok",
                Responses = ImmutableList.Create
                (
                    new DhlResponseDto
                    {
                        ErrorText = "Ok",
                        Key = "123",
                        StatusMessages = ImmutableHashSet.Create("All good"),
                        ErrorCode = 0
                    }
                )
            };

            var actual = Mapper.Map(data);

            Assert.True(actual.Count() == 1);
            Assert.True(actual.First().Id == "123");
            Assert.True(actual.First().CheckStatus == InfoPackCheckStatus.Valid);
            Output.WriteLine(actual.First().Message);
        }

        [Fact]
        public void Will_map_data_marked_as_invalid_by_online_service()
        {
            var data = new DhlMainResponseDto()
            {
                MainFaultCode = "11",
                MainFaultText = "Test error",
                Responses = ImmutableList.Create
                (
                    new DhlResponseDto
                    {
                        ErrorText = "Test Error",
                        Key = "123",
                        StatusMessages = ImmutableHashSet.Create("Test error message"),
                        ErrorCode = 11
                    }
                )
            };

            var actual = Mapper.Map(data);

            Assert.True(actual.Count() == 1);
            Assert.True(actual.First().Id == "123");
            Assert.True(actual.First().CheckStatus == InfoPackCheckStatus.Invalid);
            Output.WriteLine(actual.First().Message);
        }

        [Fact]
        public void Will_map_multiple_correct_data()
        {
            var data = new DhlMainResponseDto()
            {
                MainFaultCode = "0",
                MainFaultText = "ok",
                Responses = ImmutableList.Create
                (
                    new DhlResponseDto
                    {
                        ErrorText = "Ok",
                        Key = "123",
                        StatusMessages = ImmutableHashSet.Create("All good"),
                        ErrorCode = 0
                    },
                    new DhlResponseDto
                    {
                        ErrorText = "Ok",
                        Key = "456",
                        StatusMessages = ImmutableHashSet.Create("All good"),
                        ErrorCode = 0
                    }
                )
            };

            var actual = Mapper.Map(data);

            Assert.True(actual.Count() == 2);
            Assert.Collection(actual, d => Assert.Equal("123", d.Id), d => Assert.Equal("456", d.Id));
            Assert.All(actual, d => Assert.True(d.CheckStatus == InfoPackCheckStatus.Valid));
            Output.WriteLine(actual.First().Message);
            Output.WriteLine(actual.ElementAt(1).Message);
        }

        [Fact]
        public void Will_throw_arg_null_exc_when_trying_to_map_null()
        {
            Assert.Throws<ArgumentNullException>(() => Mapper.Map(null as DhlMainResponseDto));
        }
    }
}