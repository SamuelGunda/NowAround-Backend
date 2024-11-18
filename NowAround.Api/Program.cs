using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using NowAround.Api.Database;
using NowAround.Api.Extensions;
using NowAround.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
    });
    
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerSecurity();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddProblemDetails();

builder.Services.AddCustomServices();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddCustomCors();
builder.Services.AddApplicationInsightsTelemetry();

builder.Logging.AddLogging();

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