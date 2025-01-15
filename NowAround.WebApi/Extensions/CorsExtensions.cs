namespace NowAround.WebApi.Extensions;

public static class CorsExtensions
{
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                policyBuilder => policyBuilder
                    .WithOrigins("http://localhost:4200", "https://nowaround.site")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
        });
    }
}