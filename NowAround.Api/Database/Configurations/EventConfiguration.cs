using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Database.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired();
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.Price).IsRequired();
        builder.Property(e => e.EventPriceCategory).IsRequired();
        builder.Property(e => e.City).IsRequired();
        builder.Property(e => e.Address).IsRequired();
        builder.Property(e => e.Latitude).IsRequired();
        builder.Property(e => e.Longitude).IsRequired();
        builder.Property(e => e.MaxParticipants);
        builder.Property(e => e.PictureUrl);
        builder.Property(e => e.Start).IsRequired();
        builder.Property(e => e.End).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
        
        builder.HasOne(e => e.Establishment)
            .WithMany(est => est.Events)
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.InterestedUsers)
            .WithMany(u => u.InterestedInEvents)
            .UsingEntity<Dictionary<string, object>>(
                "EventInterest",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Event>().WithMany().HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade));

        builder.Property(e => e.EventCategory)
            .HasDefaultValue(EventCategory.Other);
    }
}