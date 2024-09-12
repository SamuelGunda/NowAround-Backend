using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    
    /*public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}