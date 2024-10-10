using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.SkName).IsRequired();
        builder.Property(c => c.Icon).IsRequired();
        
        builder.HasMany(c => c.EstablishmentCategories)
            .WithOne(ec => ec.Category)
            .HasForeignKey(ec => ec.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(c => c.Tags)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}