using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();
        
        builder.HasMany(t => t.EstablishmentTags)
            .WithOne(et => et.Tag)
            .HasForeignKey(et => et.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}