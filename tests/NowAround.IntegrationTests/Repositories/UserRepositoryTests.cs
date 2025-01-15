using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Context;
using NowAround.Infrastructure.Repository.Specific;

namespace NowAround.IntegrationTests.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<DbSet<User>> _mockDbSet;
    private readonly Mock<ILogger<User>> _mockLogger;
    
    private readonly UserRepository _repository;
}