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
    public class DataPackRepository : IDataPackRepository
    {
        private readonly IDataPackDbContext _context;

        public DataPackRepository(IDataPackDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// <inheritdoc cref="IDataPackRepository.GetDataPacks(CancellationToken)"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Set of <see cref="DataPack"/></returns>
        public Task<List<DataPack>> GetDataPacks(CancellationToken token)
        {
            return _context.DataPacks
                .OrderBy(e => e.PostIdent)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackRepository.GetParcelsSentBy(Carrier[], CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="postIdents">List of PostIdents by which <see cref="DataPack"/> will be retrieved</param>
        /// <returns>Set of <see cref="DataPack"/></returns>
        public Task<List<DataPack>> GetDataPacks(string[] postIdents, CancellationToken token)
        {
            if (postIdents?.Any(string.IsNullOrWhiteSpace) ?? true)
                throw new ArgumentOutOfRangeException(nameof(postIdents), "One or more passed in PostIdent numbers is null or empty");

            return _context.DataPacks
                .Where(d => postIdents.Contains(d.PostIdent))
                .OrderBy(e => e.PostIdent)
                .ToListAsync(token);
        }

        /// <inheritdoc cref="IDataPackRepository.GetParcelsSentBy(Carrier, CancellationToken)"/>
        /// <param name="token">Cancellation token</param>
        /// <param name="postIdent">PostIdent by which returned <see cref="DataPack"/> will be selected</param>
        /// <returns>Set of <see cref="DataPack"/></returns>
        public Task<List<DataPack>> GetDataPack(string postIdent, CancellationToken token)
        {
            return GetDataPacks(new[] { postIdent }, token);
        }

        /// <inheritdoc cref="IDataPackRepository.UpdateDataPacksTableAsync"/>
        /// <returns>Number of saved <see cref="DataPack"/></returns>
        public Task<int> UpdateDataPacksTableAsync()
        {
            // Not needed - parcels are tracked by EF Core
            //var p = _context.Parcels.Update(parcel);

            return _context.SaveChangesAsync();
        }
    }
}