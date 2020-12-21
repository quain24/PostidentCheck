using Postident.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.Common.Interfaces
{
    public interface IDataPackReadRepository
    {
        /// <summary>
        /// Provides all possible <see cref="DataPackReadModel"/> from database/>
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPacks(CancellationToken token);

        /// <summary>
        /// Provides a set of <see cref="DataPackReadModel"/> which has given <paramref name="postIdents"/>
        /// </summary>
        /// <param name="postIdents">PostIdents by which returned <see cref="DataPackReadModel"/> will be selected</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPacks(string[] postIdents, CancellationToken token);

        /// <summary>
        /// Provides a <see cref="DataPackReadModel"/> which has given <paramref name="postIdent"/>
        /// </summary>
        /// <param name="postIdent">PostIdent by which returned <see cref="DataPackReadModel"/> will be selected</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        Task<List<DataPackReadModel>> GetDataPack(string postIdent, CancellationToken token);
    }
}