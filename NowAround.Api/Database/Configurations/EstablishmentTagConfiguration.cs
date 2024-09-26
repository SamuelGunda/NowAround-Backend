using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class EstablishmentTagConfiguration : IEntityTypeConfiguration<EstablishmentTag>
{
    public void Configure(EntityTypeBuilder<EstablishmentTag> builder)
    {
        builder.HasKey(et => new { et.EstablishmentId, et.TagId });

        builder.HasOne(et => et.Establishment)
            .WithMany(e => e.EstablishmentTags)
            .HasForeignKey(et => et.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(et => et.Tag)
            .WithMany(t => t.EstablishmentTags)
            .HasForeignKey(et => et.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(et => et.EstablishmentId);
        builder.HasIndex(et => et.TagId);
    }
}