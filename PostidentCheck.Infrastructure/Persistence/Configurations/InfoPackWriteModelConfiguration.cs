using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postident.Core.Entities;

namespace Postident.Infrastructure.Persistence.Configurations
{
    public class InfoPackWriteModelConfiguration : IEntityTypeConfiguration<InfoPackWriteModel>
    {
        public void Configure(EntityTypeBuilder<InfoPackWriteModel> builder)
        {
            builder.ToTable("OrderAddressStatus", "om");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("orderId");
            builder.Property(e => e.CheckStatus).HasColumnName("checkStatus");
            builder.Property(e => e.Message).HasColumnName("message");
        }
    }
}