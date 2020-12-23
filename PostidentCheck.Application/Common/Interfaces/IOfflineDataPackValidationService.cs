using System.Collections.Generic;
using FluentValidation;
using Postident.Core.Entities;

namespace Postident.Application.Common.Interfaces
{
    public interface IOfflineDataPackValidationService<TInvalidResultsReturnType>
    {
        /// <summary>
        /// Filters all invalid data packs using provided in constructor <see cref="IValidator{T}"/>.<br/>
        /// Invalid data packs are converted into <see cref="InfoPackWriteModel"/> and returned in 'out' parameter <paramref name="invalidEntries"/>
        /// </summary>
        /// <param name="dataPacks">Collection of <see cref="DataPackReadModel"/> to be validated</param>
        /// <param name="invalidEntries">Collection of <see cref="InfoPackWriteModel"/> containing all of invalid <see cref="DataPackReadModel"/> translated to write models</param>
        /// /// <returns>Collection of valid <see cref="DataPackReadModel"/></returns>
        IEnumerable<DataPackReadModel> FilterOutInvalidDataPacksFrom(IList<DataPackReadModel> dataPacks, out IEnumerable<TInvalidResultsReturnType> invalidEntries);
    }
}