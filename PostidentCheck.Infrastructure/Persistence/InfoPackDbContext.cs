using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Persistence
{
    public class InfoPackDbContext : DbContext, IInfoPackDbContext
    {
        public InfoPackDbContext(DbContextOptions<InfoPackDbContext> options) : base(options)
        {
        }

        public DbSet<InfoPackWriteModel> Infos { get; set; }

        public async Task<int> SaveChangesAsync(IEnumerable<InfoPackWriteModel> infos)
        {
            Infos.UpdateRange(infos);
            return await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataPackDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}