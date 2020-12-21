using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Postident.Core.Entities;
using System.Threading.Tasks;

namespace Postident.Application.Common.Interfaces
{
    public interface IDataPackDbContext
    {
        DbSet<DataPackReadModel> DataPacks { get; }

        /// <summary>
        /// Will attempt to save changes made in <see cref="DataPackReadModel"/> objects retrieved from this instance of <see cref="IDataPackDbContext"/>
        /// </summary>
        /// <returns>Number of saved changes</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// <inheritdoc cref="DbContext.ChangeTracker"/>
        /// </summary>
        public ChangeTracker ChangeTracker { get; }
    }
}