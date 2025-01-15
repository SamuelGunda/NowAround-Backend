using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NowAround.Infrastructure.Context;
using NowAround.WebApi.Extensions;
using NowAround.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });
    
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerSecurity();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddProblemDetails();

builder.Services.AddCustomServices();
builder.Services.AddAuthentication(builder.Configuration);
/*
builder.Services.AddDatabase(builder.Configuration);
*/
builder.Services.AddCustomCors();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );
        sqlOptions.MigrationsAssembly("NowAround.Infrastructure");
    });
});

var app = builder.Build();

app.UseExceptionHandler();

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