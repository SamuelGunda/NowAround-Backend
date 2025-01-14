using NowAround.Api.Apis.Auth0.Interfaces;
using NowAround.Api.Apis.Auth0.Services;
using NowAround.Api.Apis.Mapbox.Services;
using NowAround.Api.Apis.Mapbox.Services.Interfaces;
using NowAround.Api.Models.Domain;
using NowAround.Api.Repositories;
using NowAround.Api.Repositories.Interfaces;
using NowAround.Api.Services;
using NowAround.Api.Services.Interfaces;

namespace NowAround.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ITokenService, TokenService>(); 
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEstablishmentService, EstablishmentService>();
        services.AddScoped<IAuth0Service, Auth0Service>();
        services.AddScoped<IMonthlyStatisticService, MonthlyStatisticService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IReviewService, ReviewService>();

        services.AddSingleton<IMapboxService, MapboxService>();
        services.AddSingleton<IMailService, MailService>();
        
        // Repositories
        services.AddScoped<IBaseAccountRepository<User>, UserRepository>();
        services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
        services.AddScoped<IMonthlyStatisticRepository, MonthlyStatisticRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
    }
}