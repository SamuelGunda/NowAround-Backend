using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Domain.Models;

namespace NowAround.Infrastructure.Configurations;

public class BusinessHoursConfiguration : IEntityTypeConfiguration<BusinessHours>
{
    public void Configure(EntityTypeBuilder<BusinessHours> builder)
    {
        builder.HasKey(bh => bh.Id);
        builder.Property(bh => bh.Monday).IsRequired();
        builder.Property(bh => bh.Tuesday).IsRequired();
        builder.Property(bh => bh.Wednesday).IsRequired();
        builder.Property(bh => bh.Thursday).IsRequired();
        builder.Property(bh => bh.Friday).IsRequired();
        builder.Property(bh => bh.Saturday).IsRequired();
        builder.Property(bh => bh.Sunday).IsRequired();
        
        builder.HasMany(bh => bh.BusinessHoursExceptions)
            .WithOne(bhe => bhe.BusinessHours)
            .HasForeignKey(bhe => bhe.BusinessHoursId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}