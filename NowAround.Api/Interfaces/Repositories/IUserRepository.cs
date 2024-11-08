﻿using NowAround.Api.Models.Domain;

namespace NowAround.Api.Interfaces.Repositories;

public interface IUserRepository
{
    Task<int> CreateUserAsync(User user);
}