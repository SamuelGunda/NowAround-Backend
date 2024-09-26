using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<Establishment> Establishments { get; set; }
    public DbSet<EstablishmentCategory> EstablishmentCategories { get; set; }
    public DbSet<EstablishmentTag> EstablishmentTags { get; set; }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    
    
    /*public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}