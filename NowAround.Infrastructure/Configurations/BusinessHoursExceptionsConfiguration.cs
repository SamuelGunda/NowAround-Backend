using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Domain.Models;

namespace NowAround.Infrastructure.Configurations;

public class BusinessHoursExceptionsConfiguration : IEntityTypeConfiguration<BusinessHoursException>
{
    public void Configure(EntityTypeBuilder<BusinessHoursException> builder)
    {
        builder.HasKey(bhe => bhe.Id);
        builder.Property(bhe => bhe.Date).IsRequired();
        builder.Property(bhe => bhe.Status).IsRequired();
    }
}