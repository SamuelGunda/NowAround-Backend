using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class EstablishmentCuisineConfiguration : IEntityTypeConfiguration<EstablishmentCuisine>
{
    public void Configure(EntityTypeBuilder<EstablishmentCuisine> builder)
    {
        builder.HasKey(ec => new { ec.CuisineId, ec.EstablishmentId });
        
        builder.HasOne(ec => ec.Cuisine)
            .WithMany(ec => ec.EstablishmentCuisines)
            .HasForeignKey(ec => ec.CuisineId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        builder.HasOne(ec => ec.Establishment)
            .WithMany(ec => ec.EstablishmentCuisines)
            .HasForeignKey(ec => ec.EstablishmentId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(ec => ec.CuisineId);
        builder.HasIndex(ec => ec.EstablishmentId);
    }
}