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
        builder.HasIndex(e => e.Auth0Id).IsUnique();
        
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.ProfilePictureUrl);
        builder.Property(e => e.BackgroundPictureUrl);
        builder.Property(e => e.Address).IsRequired();
        builder.Property(e => e.City).IsRequired();
        builder.Property(e => e.Latitude).IsRequired();
        builder.Property(e => e.Longitude).IsRequired();
        builder.Property(e => e.PriceCategory).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
        
        builder.HasOne(e => e.BusinessHours)
            .WithOne(bh => bh.Establishment)
            .HasForeignKey<BusinessHours>(bh => bh.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.RatingStatistic)
            .WithOne(rc => rc.Establishment)
            .HasForeignKey<RatingStatistic>(rc => rc.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.Menus)
            .WithOne(mi => mi.Establishment)
            .HasForeignKey(mi => mi.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.SocialLinks)
            .WithOne(sl => sl.Establishment)
            .HasForeignKey(sl => sl.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Categories)
            .WithMany(c => c.Establishments)
            .UsingEntity<Dictionary<string, object>>(
                "CategoryEstablishment",
                j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                j => j.HasOne<Establishment>().WithMany().HasForeignKey("EstablishmentId"));

        builder.HasMany(e => e.Tags)
            .WithMany(t => t.Establishments)
            .UsingEntity<Dictionary<string, object>>(
                "TagEstablishment",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                j => j.HasOne<Establishment>().WithMany().HasForeignKey("EstablishmentId"));

        builder.HasMany(e => e.Cuisines)
            .WithMany(c => c.Establishments)
            .UsingEntity<Dictionary<string, object>>(
                "CuisineEstablishment",
                j => j.HasOne<Cuisine>().WithMany().HasForeignKey("CuisineId"),
                j => j.HasOne<Establishment>().WithMany().HasForeignKey("EstablishmentId"));
        
        builder.HasMany(e => e.Posts)
            .WithOne(p => p.Establishment)
            .HasForeignKey(p => p.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(e => e.RequestStatus)
            .HasDefaultValue(RequestStatus.Pending);
    }
}