﻿using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    
    public CategoryRepository(AppDbContext context, ILogger<Category> logger) 
        : base(context, logger)
    {
    }
}