using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class DbContextInMemory : DbContext, IDataPackDbContext
    {
        public DbContextInMemory(DbContextOptions<DbContextInMemory> options)
        : base(options)
        {
        }

        public DbSet<DataPackReadModel> Parcels { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}