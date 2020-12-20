using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Postident.Core.Entities;

namespace Postident.Application.Common.Interfaces
{
    public interface IDataPackDbContext
    {
        DbSet<DataPack> DataPacks { get; }

        /// <summary>
        /// Will attempt to save changes made in <see cref="DataPack"/> objects retrieved from this instance of <see cref="IDataPackDbContext"/>
        /// </summary>
        /// <returns>Number of saved changes</returns>
        Task<int> SaveChangesAsync();
    }
}