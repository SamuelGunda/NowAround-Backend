using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.UnitTests.Repositories;

namespace NowAround.Api.UnitTests.Database;

public class TestAppDbContext : AppDbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<TestAccountEntity> TestAccountEntities { get; set; }
    public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}