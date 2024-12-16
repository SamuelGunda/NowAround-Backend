using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class BusinessHoursExceptionsConfiguration : IEntityTypeConfiguration<BusinessHoursException>
{
    public void Configure(EntityTypeBuilder<BusinessHoursException> builder)
    {
        builder.HasKey(bhe => bhe.Id);
        builder.Property(bhe => bhe.Date).IsRequired();
        builder.Property(bhe => bhe.Status).IsRequired();
    }
}