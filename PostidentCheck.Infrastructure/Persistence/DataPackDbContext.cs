using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;

namespace Postident.Infrastructure.Persistence
{
    public class DataPackDbContext : DbContext, IDataPackDbContext
    {
        public DataPackDbContext(DbContextOptions<DataPackDbContext> options) : base(options)
        {
        }

        public DbSet<DataPackReadModel> DataPacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataPackDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}