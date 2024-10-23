using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Database.Configurations;

public class EstablishmentConfiguration : IEntityTypeConfiguration<Establishment>
{
    public void Configure(EntityTypeBuilder<Establishment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Auth0Id).IsRequired();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.Address).IsRequired();
        builder.Property(e => e.City).IsRequired();
        builder.Property(e => e.Website);
        builder.Property(e => e.Latitude).IsRequired();
        builder.Property(e => e.Longitude).IsRequired();
        builder.Property(e => e.PriceCategory).IsRequired();
        
        builder.HasMany(e => e.EstablishmentCategories)
            .WithOne(ec => ec.Establishment)
            .HasForeignKey(ec => ec.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.EstablishmentTags)
            .WithOne(et => et.Establishment)
            .HasForeignKey(et => et.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.RequestStatus)
            .HasDefaultValue((RequestStatus) 0);
    }
}