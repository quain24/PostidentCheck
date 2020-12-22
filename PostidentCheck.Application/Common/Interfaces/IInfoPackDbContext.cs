using Postident.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Postident.Application.Common.Interfaces
{
    public interface IInfoPackDbContext
    {
        /// <summary>
        /// Will attempt to save changes made in <see cref="InfoPackWriteModel"/> objects.
        /// </summary>
        /// <returns>Number of saved changes</returns>
        Task<int> SaveChangesAsync(IEnumerable<InfoPackWriteModel> infos);
    }
}