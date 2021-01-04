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

        public int ErrorCode { get; init; } = 9999;

        public string ErrorText { get; init; } = string.Empty;

        public ImmutableHashSet<string> StatusMessages { get; init; } = ImmutableHashSet<string>.Empty;

        /// <summary>
        /// Validation state according to online service.
        /// </summary>
        public bool IsValid => ErrorCode == 0;

        public override string ToString()
        {
            var str = string.Empty;
            if (!IsValid)
            {
                str = $"Error code: {ErrorCode} | Type: {ValueOrFiller(ErrorText)} | ";
            }
            return str += $"{ValueOrFiller(StatusMessages)}";
        }

        public static implicit operator string(DhlResponseDto response) => response.ToString();

        private static string ValueOrFiller(string value) => string.IsNullOrWhiteSpace(value) ? "---" : value;

        private static string ValueOrFiller(ImmutableHashSet<string> values) => values.IsNullOrEmpty() ? "---" : string.Join(" ", values);
    }
}