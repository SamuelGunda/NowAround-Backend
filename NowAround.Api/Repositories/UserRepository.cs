using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories.Interfaces;

namespace NowAround.Api.Repositories;

public class UserRepository : BaseAccountRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger<User> logger) 
        : base(context, logger)
    {
    }
}