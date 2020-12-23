using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class InfoPacksDbContextInMemory : DbContext, IInfoPackDbContext
    {
        public InfoPacksDbContextInMemory(DbContextOptions<InfoPacksDbContextInMemory> options)
        : base(options)
        {
        }
        public DbSet<DataPackReadModel> InfoPacks { get; }


        public Task<int> SaveChangesAsync(IEnumerable<InfoPackWriteModel> infos)
        {
            base.UpdateRange(infos);
            return SaveChangesAsync();
        }
    }
}