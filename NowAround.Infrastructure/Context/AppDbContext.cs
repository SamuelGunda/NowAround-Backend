﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NowAround.Domain.Enum;
using NowAround.Domain.Models;

namespace NowAround.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<MonthlyStatistic> MonthlyStatistics { get; set; }
    public DbSet<Establishment> Establishments { get; set; }

    public DbSet<BusinessHours> BusinessHours { get; set; }
    public DbSet<BusinessHoursException> BusinessHoursExceptions { get; set; }

    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }

    public DbSet<SocialLink> SocialLinks { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Review> Reviews { get; set; }
    public DbSet<RatingStatistic> RatingStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Establishment>().HasQueryFilter(e => e.RequestStatus == RequestStatus.Accepted);
    }
}