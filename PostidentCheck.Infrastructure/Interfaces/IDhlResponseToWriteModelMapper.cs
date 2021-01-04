using Postident.Application.DHL;
using Postident.Core.Entities;
using System.Collections.Generic;

namespace Postident.Infrastructure.Interfaces
{
    public interface IDhlResponseToWriteModelMapper
    {
        IEnumerable<InfoPackWriteModel> Map(DhlMainResponseDto response);

        IEnumerable<InfoPackWriteModel> Map(IEnumerable<DhlMainResponseDto> responses);
    }
}