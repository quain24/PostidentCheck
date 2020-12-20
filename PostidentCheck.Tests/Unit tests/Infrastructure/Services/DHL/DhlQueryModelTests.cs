using System;
using System.Linq;
using Postident.Infrastructure.Services.DHL;
using Xunit;

namespace Postident.Tests.Unit_tests.Infrastructure.Services.DHL
{
    public class DhlQueryModelTests
    {
        private readonly DhlApiQueryModel QueryModel;

        public DhlQueryModelTests()
        {
            QueryModel = new DhlApiQueryModel();
        }

        [Fact()]
        public void After_first_parcel_tracking_number_add_will_not_have_separator_after_piece_code()
        {
            var pieceCode = "ABC123456";
            QueryModel.AddParcelTrackingNumber(pieceCode);

            Assert.Equal(pieceCode, QueryModel.ParcelTrackingNumbers);
        }

        [Theory]
        [InlineData("ABC123456;123ABC;AAAAAAA", new[] { "ABC123456", "123ABC", "AAAAAAA" })]
        public void After_two_or_more_parcel_tracking_numbers_were_added_should_get_those_codes_separated_by_semicolon(string expected, string[] input)
        {
            input.ToList().ForEach(s => QueryModel.AddParcelTrackingNumber(s));

            Assert.Equal(expected, QueryModel.ParcelTrackingNumbers);
        }

        [Fact]
        public void Asked_for_parcel_tracking_number_will_return_empty_string_when_no_piece_data()
        {
            Assert.True(QueryModel.ParcelTrackingNumbers?.Length == 0);
        }

        [Theory]
        [InlineData("ABC123456;123ABC;AAAAAAA", new[] { "ABC123456", "123ABC", "AAAAAAA" })]
        public void Add_pieces_will_combine_parcel_tracking_numbers_with_separator_in_between(string expected, string[] input)
        {
            QueryModel.AddParcelTrackingNumbers(input);

            Assert.Equal(QueryModel.ParcelTrackingNumbers, expected);
        }

        [Theory]
        [InlineData(new object[] { new[] { "ABC123456", "", "AAAAAAA" } })]
        [InlineData(new object[] { new[] { "ABC123456", "    ", "AAAAAAA" } })]
        [InlineData(new object[] { new[] { "ABC123456", null, "AAAAAAA" } })]
        public void Add_pieces_will_throw_ArgumentOutOfRange_if_feed_with_empty_parcel_tracking_number(string[] input)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => QueryModel.AddParcelTrackingNumbers(input));
        }

        [Theory]
        [InlineData("ABC123456;123ABC;AAAAAAA", new[] { "    ABC123456", "   123ABC", "AAAAAAA" })]
        [InlineData("ABC123456;123ABC;AAAAAAA", new[] { "    ABC123456", "   123ABC", "AAAAAAA     " })]
        [InlineData("ABC123456;123ABC;AAAAAAA", new[] { "ABC123456", "123ABC   ", "AAAAAAA     " })]
        public void Add_pieces_will_remove_unnecessary_whitespaces_in_front_and_back_from_parcel_tracking_numbers(string expected, string[] input)
        {
            QueryModel.AddParcelTrackingNumbers(input);

            var actual = QueryModel.ParcelTrackingNumbers;

            Assert.Equal(expected, actual);
        }
    }
}