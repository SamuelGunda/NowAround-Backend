using Microsoft.EntityFrameworkCore;
using NowAround.Api.Database;

namespace NowAround.Api.Extensions;

public static class DbContextExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration
                    .GetSection("ConnectionStrings")
                    .GetValue<string>("default"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5, 
                    maxRetryDelay: TimeSpan.FromSeconds(30), 
                    errorNumbersToAdd: null
                )
            );
        });
    }
}