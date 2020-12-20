using Postident.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.Common.Interfaces
{
    public interface IDataPackRepository
    {
        /// <summary>
        /// Provides all possible <see cref="DataPack"/> from database/>
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPack"/></returns>
        Task<List<DataPack>> GetDataPacks(CancellationToken token);

        /// <summary>
        /// Provides a set of <see cref="DataPack"/> which has given <paramref name="postIdents"/>
        /// </summary>
        /// <param name="postIdents">PostIdents by which returned <see cref="DataPack"/> will be selected</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPack"/></returns>
        Task<List<DataPack>> GetDataPacks(string[] postIdents, CancellationToken token);

        /// <summary>
        /// Provides a <see cref="DataPack"/> which has given <paramref name="postIdent"/>
        /// </summary>
        /// <param name="postIdent">PostIdent by which returned <see cref="DataPack"/> will be selected</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPack"/></returns>
        Task<List<DataPack>> GetDataPack(string postIdent, CancellationToken token);

        /// <summary>
        /// Saves changes made in <see cref="DataPack"/> objects that originated from matching instance of <see cref="IDataPackDbContext"/>
        /// </summary>
        /// <returns>Number of saved changes</returns>
        Task<int> UpdateDataPacksTableAsync();
    }
}