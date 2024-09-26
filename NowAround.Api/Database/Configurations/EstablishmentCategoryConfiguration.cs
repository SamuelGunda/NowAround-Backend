using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class EstablishmentCategoryConfiguration : IEntityTypeConfiguration<EstablishmentCategory>
{
    public void Configure(EntityTypeBuilder<EstablishmentCategory> builder)
    {
        builder.HasKey(ec => new { ec.EstablishmentId, ec.CategoryId });

        builder.HasOne(ec => ec.Establishment)
            .WithMany(e => e.EstablishmentCategories)
            .HasForeignKey(ec => ec.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ec => ec.Category)
            .WithMany(c => c.EstablishmentCategories)
            .HasForeignKey(ec => ec.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ec => ec.EstablishmentId);
        builder.HasIndex(ec => ec.CategoryId);
    }
}