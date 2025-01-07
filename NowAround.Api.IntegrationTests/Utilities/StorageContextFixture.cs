using Microsoft.Extensions.Configuration;
using NowAround.Api.Utilities;

namespace NowAround.Api.IntegrationTests;

public class StorageContextFixture : IDisposable
{
    public StorageContext StorageContext { get; }

    public StorageContextFixture()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        Console.WriteLine($"Environment: {environment}");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        StorageContext = new StorageContext(configuration);
    }

    public void Dispose()
    {
    }
}
