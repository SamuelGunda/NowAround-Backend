using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class RatingStatisticConfiguration : IEntityTypeConfiguration<RatingStatistic>
{
    public void Configure(EntityTypeBuilder<RatingStatistic> builder)
    {
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.OneStar).IsRequired();
        builder.Property(rc => rc.TwoStars).IsRequired();
        builder.Property(rc => rc.ThreeStars).IsRequired();
        builder.Property(rc => rc.FourStars).IsRequired();
        builder.Property(rc => rc.FiveStars).IsRequired();
        
        builder.HasMany(rc => rc.Reviews)
            .WithOne(r => r.RatingStatistic)
            .HasForeignKey(r => r.RatingCollectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}