using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace NowAround.Infrastructure.Context;

public class StorageContext
{
    public BlobServiceClient BlobServiceClient { get; }

    public StorageContext(IConfiguration configuration)
    {
        var storageKey = configuration.GetConnectionString("StorageKey") ?? throw new ArgumentNullException(nameof(configuration), "StorageKey is missing");
        var storageAccount = configuration.GetConnectionString("StorageAccount") ?? throw new ArgumentNullException(nameof(configuration), "StorageAccount is missing");

        var credential = new StorageSharedKeyCredential(storageAccount, storageKey);
        BlobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccount}.blob.core.windows.net"), credential);
    }
}