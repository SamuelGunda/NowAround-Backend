using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class RegisterStatisticConfiguration : IEntityTypeConfiguration<MonthlyStatistic>
{
    public void Configure(EntityTypeBuilder<MonthlyStatistic> builder)
    {
        builder.HasKey(rs => rs.Date);
        builder.Property(rs => rs.UsersCreatedCount);
        builder.Property(rs => rs.EstablishmentsCreatedCount);
    }
}