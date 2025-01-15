using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Domain.Models;

namespace NowAround.Infrastructure.Configurations;

public class SocialLinkConfiguration : IEntityTypeConfiguration<SocialLink>
{
    public void Configure(EntityTypeBuilder<SocialLink> builder)
    {
        builder.HasKey(sl => sl.Id);
        builder.Property(sl => sl.Name).IsRequired();
        builder.Property(sl => sl.Url).IsRequired();
    }
}