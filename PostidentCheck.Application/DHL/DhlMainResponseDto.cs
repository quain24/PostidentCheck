using System.Collections.Immutable;

namespace Postident.Application.DHL
{
    public class DhlMainResponseDto
    {
        public string MainFaultCode { get; init; } = string.Empty;

        public string MainFaultText { get; init; } = string.Empty;

        public ImmutableList<DhlResponseDto> Responses { get; init; } = ImmutableList<DhlResponseDto>.Empty;
    }
}