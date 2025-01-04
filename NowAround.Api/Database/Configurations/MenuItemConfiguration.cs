using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(mi => mi.Id);
        
        builder.Property(mi => mi.Name)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(mi => mi.PictureUrl)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(mi => mi.Description)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(mi => mi.Price)
            .IsRequired()
            .HasMaxLength(16);
    }
}