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
        builder.Property(p => p.PictureUrl);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
        
        builder.HasOne(p => p.Establishment)
            .WithMany(e => e.Posts)
            .HasForeignKey(p => p.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.Likes)
            .WithMany(u => u.LikedPosts)
            .UsingEntity<Dictionary<string, object>>(
                "PostLike",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Post>().WithMany().HasForeignKey("PostId").OnDelete(DeleteBehavior.Cascade));
    }
}