using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class InfoPacksDbContextInMemory : DbContext, IInfoPackDbContext
    {
        public InfoPacksDbContextInMemory(DbContextOptions<InfoPacksDbContextInMemory> options)
        : base(options)
        {
        }

        public DbSet<InfoPackWriteModel> InfoPacks { get; set; }

        public Task<int> SaveChangesAsync(IEnumerable<InfoPackWriteModel> infos)
        {
            base.UpdateRange(infos);
            return SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InfoPackWriteModel>().Property(p => p.CheckStatus).HasConversion<int>();
            base.OnModelCreating(modelBuilder);
        }
    }
}