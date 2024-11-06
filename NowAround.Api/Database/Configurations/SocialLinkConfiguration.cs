using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class SocialLinkConfiguration : IEntityTypeConfiguration<SocialLink>
{
    public void Configure(EntityTypeBuilder<SocialLink> builder)
    {
        builder.HasKey(sl => sl.Id);
        builder.Property(sl => sl.Name).IsRequired();
        builder.Property(sl => sl.Url).IsRequired();
        
        builder.HasOne(sl => sl.Establishment)
            .WithMany(e => e.SocialLinks)
            .HasForeignKey(sl => sl.EstablishmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}