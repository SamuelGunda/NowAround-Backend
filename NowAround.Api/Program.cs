using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NowAround.Api.Authentication.Interfaces;
using NowAround.Api.Authentication.Repositories;
using NowAround.Api.Authentication.Services;
using NowAround.Api.Database;
using NowAround.Api.Interfaces;
using NowAround.Api.Interfaces.Repositories;
using NowAround.Api.Repositories;
using NowAround.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
    });
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
        );
});

builder.Configuration.AddEnvironmentVariables();

var auth0ClientId = builder.Configuration["Auth0:ClientId"] ?? throw new InvalidOperationException("Auth0 ClientId is missing");
var auth0ClientSecret = builder.Configuration["Auth0:ClientSecret"] ?? throw new InvalidOperationException("Auth0 ClientSecret is missing");
var auth0Domain = builder.Configuration["Auth0:Domain"] ?? throw new InvalidOperationException("Auth0 Domain is missing");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Domain}/";
        options.Audience = "https://now-around-auth-api/";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
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

builder.Services.AddScoped<ITokenService, TokenService>();  
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEstablishmentService, EstablishmentService>();
builder.Services.AddScoped<IMapboxService, MapboxService>();
builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();

builder.Services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration
        .GetSection("ConnectionStrings")
        .GetValue<string>("default"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, 
            maxRetryDelay: TimeSpan.FromSeconds(30), 
            errorNumbersToAdd: null
        )
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Wake up the database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.ExecuteSqlRaw("SELECT 1");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error waking up the database: {ex.Message}");
    }
}

app.Run();