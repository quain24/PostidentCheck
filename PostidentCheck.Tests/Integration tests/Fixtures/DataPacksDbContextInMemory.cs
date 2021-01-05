using Microsoft.EntityFrameworkCore;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using Postident.Core.Enums;
using System;
using System.Globalization;

namespace Postident.Tests.Integration_tests.Fixtures
{
    public class DataPacksDbContextInMemory : DbContext, IDataPackDbContext
    {
        public DataPacksDbContextInMemory(DbContextOptions<DataPacksDbContextInMemory> options)
        : base(options)
        {
        }

        public DbSet<DataPackReadModel> DataPacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataPackReadModel>().Property(p => p.Carrier)
                .HasConversion(c => ((int)c).ToString(NumberFormatInfo.InvariantInfo), s => Enum.Parse<Carrier>(s));
            modelBuilder.Entity<DataPackReadModel>().Property(p => p.DataPackChecked).HasConversion<int>();
            base.OnModelCreating(modelBuilder);
        }
    }
}