using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Repositories;
using NowAround.Api.Authentication.Services;
using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Repositories;
using NowAround.Api.Services;

namespace NowAround.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ITokenService, TokenService>();  
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEstablishmentService, EstablishmentService>();
        services.AddScoped<IMapboxService, MapboxService>();
        services.AddScoped<IAccountManagementService, AccountManagementService>();

        // Repositories
        services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
    }
}