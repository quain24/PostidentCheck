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
        /// <inheritdoc cref="DbContext.ChangeTracker"/>
        /// </summary>
        public ChangeTracker ChangeTracker { get; }
    }
}