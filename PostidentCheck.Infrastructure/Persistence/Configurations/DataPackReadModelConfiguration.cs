using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postident.Core.Entities;

namespace Postident.Infrastructure.Persistence.Configurations
{
    internal class DataPackReadModelConfiguration : IEntityTypeConfiguration<DataPackReadModel>
    {
        public void Configure(EntityTypeBuilder<DataPackReadModel> builder)
        {
            // todo Table / view must have some sort of key!! Add additional column with unique key.
            //builder.HasKey(e => e.ParcelId);

            builder.ToView("GetPostidentsToCheck");

            builder.Property(e => e.City).HasColumnName("city");
            builder.Property(e => e.Street).HasColumnName("street");
            builder.Property(e => e.ZipCode).HasColumnName("zip");
            builder.Property(e => e.CountryCode).HasColumnName("country");
            builder.Property(e => e.PostIdent).HasColumnName("postIdent");

            // todo This property needs a column in view to save some error informations (varchar)
            builder.Ignore(e => e.ErrorInfo);

            // todo This should have an actual column, otherwise where the data will be saved?
            builder.Property(e => e.PostIdentChecked).HasColumnName("postIdentChecked");
        }
    }
}