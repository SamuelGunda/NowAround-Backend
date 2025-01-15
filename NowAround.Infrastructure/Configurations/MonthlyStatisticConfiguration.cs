using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Domain.Models;

namespace NowAround.Infrastructure.Configurations;

public class RegisterStatisticConfiguration : IEntityTypeConfiguration<MonthlyStatistic>
{
    public void Configure(EntityTypeBuilder<MonthlyStatistic> builder)
    {
        builder.HasKey(rs => rs.Date);
        builder.Property(rs => rs.UsersCreatedCount);
        builder.Property(rs => rs.EstablishmentsCreatedCount);
    }
}