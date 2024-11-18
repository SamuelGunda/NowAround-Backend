﻿using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category?> GetByNameWithTagsAsync(string name);
}