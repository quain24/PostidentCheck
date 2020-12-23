using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using Postident.Core.Enums;
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
                .OrderBy(e => e.Id)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackReadRepository.GetParcelsSentBy(Carrier[], CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="ids">List of ID's by which <see cref="DataPackReadModel"/> will be retrieved</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPacks(string[] ids, CancellationToken token)
        {
            if (ids?.Any(string.IsNullOrWhiteSpace) ?? true)
                throw new ArgumentOutOfRangeException(nameof(ids), "One or more passed in id numbers is null or empty");

            var idsUnique = ids.ToHashSet();

            return _context.DataPacks
                .Where(d => idsUnique.Contains(d.Id))
                .OrderBy(e => e.Id)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackReadRepository.GetDataPacks(Carrier, CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="id">ID by which single returned <see cref="DataPackReadModel"/> will be selected</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPack(string id, CancellationToken token)
        {
            return GetDataPacks(new[] { id }, token);
        }

        /// <summary>
        /// <inheritdoc cref="IDataPackReadRepository.GetDataPacks(Carrier[], CancellationToken)"/>
        /// </summary>
        /// <param name="carriers">Carriers list by which database will be queued</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Set of <see cref="DataPackReadModel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPacks(Carrier[] carriers, CancellationToken token)
        {
            if (carriers?.Any(c => !Enum.IsDefined(typeof(Carrier), c)) ?? true)
            {
                throw new ArgumentOutOfRangeException(nameof(carriers),
                    $"One or more passed in carrier types is unknown - no such value in {nameof(Carrier)} enum or null was passed");
            }

            var carriersUnique = carriers.ToHashSet();

            return _context.DataPacks
                .Where(p => carriersUnique.Contains(p.Carrier))
                .OrderBy(e => e.Id)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackReadRepository.GetDataPacks(Carrier, CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="carrier">Carriers list by which database will be queued</param>
        /// <returns>Set of <see cref="Parcel"/></returns>
        public Task<List<DataPackReadModel>> GetDataPacks(Carrier carrier, CancellationToken token)
        {
            return GetDataPacks(new[] { carrier }, token);
        }
    }
}