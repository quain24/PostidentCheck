using Postident.Core.Entities;
using Postident.Core.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.Common.Interfaces
{
    public interface IDataPackReadRepository
    {
        /// <summary>
        /// Provides all possible <see cref="DataPackReadModel"/> from database.
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPacks(CancellationToken token);

        /// <summary>
        /// Provides a set of <see cref="DataPackReadModel"/> which has given <paramref name="postIdents"/>
        /// </summary>
        /// <param name="ids">ID's by which returned <see cref="DataPackReadModel"/> will be selected</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPacks(string[] ids, CancellationToken token);

        /// <summary>
        /// Provides a <see cref="DataPackReadModel"/> which has given <paramref name="postIdent"/>
        /// </summary>
        /// <param name="id">ID by which single returned <see cref="DataPackReadModel"/> will be selected</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPack(string id, CancellationToken token);

        /// <summary>
        /// Provides all orders send by requested parcel services.
        /// </summary>
        /// <param name="carriers">Carriers list by which database will be queued</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPacks(Carrier[] carriers, CancellationToken token);

        /// <summary>
        /// Provides all orders send by requested parcel service.
        /// </summary>
        /// <param name="carrier">Carriers list by which database will be queued</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPacks(Carrier carrier, CancellationToken token);
    }
}