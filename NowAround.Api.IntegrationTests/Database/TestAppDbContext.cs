using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.IntegrationTests.Repositories;

namespace NowAround.Api.IntegrationTests.Database;

public class TestAppDbContext : AppDbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<TestAccountEntity> TestAccountEntities { get; set; }
    public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}