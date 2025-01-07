﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Database.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(mi => mi.Id);
        builder.Property(mi => mi.Name).IsRequired();
        builder.Property(mi => mi.Description).IsRequired();
        builder.Property(mi => mi.Price).IsRequired();
        builder.Property(mi => mi.PictureUrl);
    }
}