using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Headline).IsRequired();
        builder.Property(p => p.Body).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
        
        builder.HasOne(p => p.Establishment)
            .WithMany(e => e.Posts)
            .HasForeignKey(p => p.EstablishmentId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(p => p.PostLikes)
            .WithOne(pl => pl.Post)
            .HasForeignKey(pl => pl.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}