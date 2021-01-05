using Postident.Application.Common.Models;
using Postident.Core.Entities;
using System;
using System.Collections.Generic;

namespace Postident.Application.Common.Interfaces
{
    public interface IReadModelToDataPackMapper
    {
        /// <summary>
        /// Will try to map given <see cref="DataPackReadModel"/> into <see cref="DataPack"/> - more usable version of data from DB.
        /// </summary>
        /// <param name="dataPackReadModel">Entity from database</param>
        /// <returns><see cref="DataPack"/> created from provided <paramref name="dataPackReadModel"/></returns>
        /// <exception cref="ArgumentNullException">When trying to map <see langword="null"/></exception>
        DataPack Map(DataPackReadModel dataPackReadModel);

        /// <summary>
        /// Will try to map given <see cref="DataPackReadModel"/> collection into a collection of <see cref="DataPack"/> - more usable version of data from DB.
        /// </summary>
        /// <param name="dataPackReadModels">Entities from database</param>
        /// <returns>Collection of <see cref="DataPack"/> created from provided <paramref name="dataPackReadModels"/></returns>
        /// <exception cref="ArgumentNullException">When trying to map <see langword="null"/></exception>
        IEnumerable<DataPack> Map(IEnumerable<DataPackReadModel> dataPackReadModels);
    }
}