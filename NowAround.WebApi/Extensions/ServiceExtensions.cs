using NowAround.Application.Common.Helpers;
using NowAround.Application.Interfaces;
using NowAround.Application.Services;
using NowAround.Domain.Interfaces.Base;
using NowAround.Domain.Interfaces.Specific;
using NowAround.Domain.Models;
using NowAround.Infrastructure.Repository.Base;
using NowAround.Infrastructure.Repository.Specific;
using NowAround.Infrastructure.Services.Auth0;
using NowAround.Infrastructure.Services.Mapbox;

namespace NowAround.WebApi.Extensions;

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