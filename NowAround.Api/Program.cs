using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NowAround.Api.Database;
using NowAround.Api.Extensions;

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

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCustomServices();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.AddApplicationInsightsTelemetry();

builder.Logging.ConfigureLogging();

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