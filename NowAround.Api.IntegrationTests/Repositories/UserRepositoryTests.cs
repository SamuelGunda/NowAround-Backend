using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NowAround.Api.Database;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;

namespace NowAround.Api.IntegrationTests.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<DbSet<User>> _mockDbSet;
    private readonly Mock<ILogger<User>> _mockLogger;
    
    private readonly UserRepository _repository;
}