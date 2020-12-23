using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class DataPacksDbContextInMemory : DbContext, IDataPackDbContext
    {
        public DataPacksDbContextInMemory(DbContextOptions<DataPacksDbContextInMemory> options)
        : base(options)
        {
        }
        public DbSet<DataPackReadModel> DataPacks { get; }
        
        
    }
}