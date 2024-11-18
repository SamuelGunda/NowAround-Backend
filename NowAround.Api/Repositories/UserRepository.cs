using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;

namespace NowAround.Api.Repositories;

public interface IUserRepository : IBaseAccountRepository<User>
{
}

public class UserRepository : BaseAccountRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger<User> logger) 
        : base(context, logger)
    {
    }
}