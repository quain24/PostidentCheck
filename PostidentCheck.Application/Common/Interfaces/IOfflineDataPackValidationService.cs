using FluentValidation;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using System.Collections.Generic;

namespace Postident.Application.Common.Interfaces
{
    public interface IOfflineDataPackValidationService<TInvalidResultsReturnType>
    {
        /// <summary>
        /// Filters all invalid data packs using <see cref="IValidator{T}"/> provided in constructor.<br/>
        /// Data without ID is logged and thrown away, since it cannot be processed any further.<br/>
        /// Invalid data packs are converted into <see cref="InfoPackWriteModel"/> and returned in 'out' parameter <paramref name="invalidEntries"/>
        /// </summary>
        /// <param name="dataPacks">Collection of <see cref="DataPack"/> to be validated</param>
        /// <param name="invalidEntries">Collection of <see cref="InfoPackWriteModel"/> containing all of invalid <see cref="DataPack"/> translated to write models</param>
        /// <returns>Collection of valid <see cref="DataPack"/></returns>
        /// <exception cref="System.ArgumentNullException">When <paramref name="dataPacks"/> is <see langword="null"/></exception>
        IEnumerable<DataPack> FilterOutInvalidDataPacksFrom(IList<DataPack> dataPacks, out IEnumerable<TInvalidResultsReturnType> invalidEntries);
    }
}