using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class BusinessHoursConfiguration : IEntityTypeConfiguration<BusinessHours>
{
    public void Configure(EntityTypeBuilder<BusinessHours> builder)
    {
        builder.HasKey(bh => bh.Id);

        builder.Property(bh => bh.Monday)
            .IsRequired()
            .HasMaxLength(48);

        builder.Property(bh => bh.Tuesday)
            .IsRequired()
            .HasMaxLength(48);

        builder.Property(bh => bh.Wednesday)
            .IsRequired()
            .HasMaxLength(48);

        builder.Property(bh => bh.Thursday)
            .IsRequired()
            .HasMaxLength(48);

        builder.Property(bh => bh.Friday)
            .IsRequired()
            .HasMaxLength(48);

        builder.Property(bh => bh.Saturday)
            .IsRequired()
            .HasMaxLength(48);

        builder.Property(bh => bh.Sunday)
            .IsRequired()
            .HasMaxLength(48);
        
        builder.HasMany(bh => bh.BusinessHoursExceptions)
            .WithOne(bhe => bhe.BusinessHours)
            .HasForeignKey(bhe => bhe.BusinessHoursId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}