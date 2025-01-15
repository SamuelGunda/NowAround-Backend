using Microsoft.EntityFrameworkCore;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.UnitTests.Repository.Base;
using NowAround.IntegrationTests.Repositories;

namespace NowAround.IntegrationTests;

public class TestAppDbContext : AppDbContext
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<TestAccountEntity> TestAccountEntities { get; set; }
    public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}