using SharedExtensions;
using System.Collections.Immutable;

namespace Postident.Application.DHL
{
    /// <summary>
    /// Response from <see cref="Core.Enums.Carrier.DHL"/> API.
    /// </summary>
    public sealed class DhlResponseDto
    {
        /// <summary>
        /// A key corresponding to main index of entity that will be used with it main index.
        /// </summary>
        public string Key { get; init; } = string.Empty;

        public string ErrorCode { get; init; } = string.Empty;

        public string ErrorText { get; init; } = string.Empty;

        public ImmutableHashSet<string> StatusMessages { get; init; } = ImmutableHashSet<string>.Empty;

        public override string ToString()
        {
            return $"Error code: {ValueOrFiller(ErrorCode)} | " + $"Error text: {ValueOrFiller(ErrorText)} | Status messages(s): {ValueOrFiller(StatusMessages)}";
        }

        public static implicit operator string(DhlResponseDto response) => response.ToString();

        private static string ValueOrFiller(string value) => string.IsNullOrWhiteSpace(value) ? "---" : value;

        private static string ValueOrFiller(ImmutableHashSet<string> values) => values.IsNullOrEmpty() ? "---" : string.Join(" & ", values);
    }
}