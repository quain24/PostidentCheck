using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postident.Core.Entities;
using Postident.Core.Enums;
using System;

namespace Postident.Infrastructure.Persistence.Configurations
{
    internal class DataPackReadModelConfiguration : IEntityTypeConfiguration<DataPackReadModel>
    {
        public void Configure(EntityTypeBuilder<DataPackReadModel> builder)
        {
            builder.ToView("GetAddressToCheck");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("orderId");
            builder.Property(e => e.Carrier).HasColumnName("carrier")
                .HasConversion(c => c.ToString(), s => Enum.Parse<Carrier>(s));
            builder.Property(e => e.City).HasColumnName("city");
            builder.Property(e => e.Street).HasColumnName("street");
            builder.Property(e => e.ZipCode).HasColumnName("zip");
            builder.Property(e => e.CountryCode).HasColumnName("country");
            builder.Property(e => e.PostIdent).HasColumnName("postIdent");
            builder.Property(e => e.DataPackChecked).HasColumnName("checkStatus");
        }
    }
}