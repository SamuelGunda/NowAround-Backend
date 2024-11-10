using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NowAround.Api.Models.Domain;
using NowAround.Api.Models.Enum;

namespace NowAround.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MonthlyStatistic> MonthlyStatistics { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<Establishment> Establishments { get; set; }
    public DbSet<EstablishmentCategory> EstablishmentCategories { get; set; }
    public DbSet<EstablishmentTag> EstablishmentTags { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    
    /*public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<Establishment>().HasQueryFilter(e => e.RequestStatus == RequestStatus.Accepted);
    }
}