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

        public string MainFaultCode { get; init; } = string.Empty;

        public string MainFaultText { get; init; } = string.Empty;

        public string ErrorCode { get; init; } = string.Empty;

        public string ErrorText { get; init; } = string.Empty;

        public ImmutableHashSet<string> StatusMessages { get; init; } = ImmutableHashSet<string>.Empty;

        public override string ToString()
        {
            return $"Main fault code: {ValueOrFiller(MainFaultCode)} | Main fault text: {ValueOrFiller(MainFaultText)} | Error code: {ValueOrFiller(ErrorCode)} | " +
                   $"Error text: {ValueOrFiller(ErrorText)} | Status messages: {ValueOrFiller(StatusMessages)}";
        }

        private static string ValueOrFiller(string value) => string.IsNullOrWhiteSpace(value) ? "---" : value;

        private string ValueOrFiller(ImmutableHashSet<string> values) => StatusMessages.IsEmpty ? "---" : string.Join(", ", StatusMessages);
    }
}