using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace NowAround.Api.Extensions;

public static class AuthenticationExtensions
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Domain = configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0 Domain is missing");
        var auth0Audience = configuration["Auth0:Audience"] ?? throw new InvalidOperationException("Auth0 Audience is missing");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://{auth0Domain}/";
            options.Audience = $"{auth0Audience}";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RoleClaimType = $"{auth0Audience}roles",
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(context.Exception, "Authentication failed.");
                    return Task.CompletedTask;
                }
            };
        });
        
        services.AddAuthorizationBuilder()
                    .AddPolicy("EstablishmentOnly", policy => policy.RequireRole("Establishment"))
                    .AddPolicy("UserOnly", policy => policy.RequireRole("User"))
                    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    }
}