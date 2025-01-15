using Microsoft.Extensions.Logging;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Base;

namespace NowAround.Infrastructure.Repository.Specific;

public class UserRepository : BaseAccountRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger<User> logger) 
        : base(context, logger)
    {
    }
}