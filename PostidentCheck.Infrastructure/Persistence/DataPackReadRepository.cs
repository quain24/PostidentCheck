using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Persistence
{
    public class DataPackReadRepository : IDataPackReadRepository
    {
        private readonly IDataPackDbContext _context;

        public DataPackReadRepository(IDataPackDbContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// <inheritdoc cref="IDataPackReadRepository.GetDataPacks(CancellationToken)"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPacks(CancellationToken token)
        {
            return _context.DataPacks
                .OrderBy(e => e.PostIdent)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackReadRepository.GetParcelsSentBy(Carrier[], CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="ids">List of PostIdents by which <see cref="DataPackReadModel"/> will be retrieved</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPacks(string[] ids, CancellationToken token)
        {
            if (ids?.Any(string.IsNullOrWhiteSpace) ?? true)
                throw new ArgumentOutOfRangeException(nameof(ids), "One or more passed in PostIdent numbers is null or empty");

            return _context.DataPacks
                .Where(d => ids.Contains(d.Id))
                .OrderBy(e => e.Id)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackReadRepository.GetParcelsSentBy(Carrier, CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="postIdent">PostIdent by which returned <see cref="DataPackReadModel"/> will be selected</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPack(string id, CancellationToken token)
        {
            return GetDataPacks(new[] { id }, token);
        }
    }
}