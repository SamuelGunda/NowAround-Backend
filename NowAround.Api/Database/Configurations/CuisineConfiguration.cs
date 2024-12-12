using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class CuisineConfiguration : IEntityTypeConfiguration<Cuisine>
{
    public void Configure(EntityTypeBuilder<Cuisine> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(32).IsRequired();
        
        builder.HasMany(c => c.EstablishmentCuisines)
            .WithOne(c => c.Cuisine)
            .HasForeignKey(c => c.CuisineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}