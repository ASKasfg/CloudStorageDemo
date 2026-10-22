namespace CloudStorageDemo.Patterns.Providers;

public sealed class AzureProviderAdapter : ICloudProvider
{
    private const string Container = "cloud-storage";
    private readonly AzureLikeProvider _azure;

    public AzureProviderAdapter(AzureLikeProvider azure)
        => _azure = azure ?? throw new ArgumentNullException(nameof(azure));

    public string Name => "Azure-like";

    public Task UploadAsync(string path, byte[] data)
    {
        using var ms = new MemoryStream(data, writable: false);
        _azure.CreateBlob(Container, path, ms);
        return Task.CompletedTask;
    }

    public async Task<byte[]> DownloadAsync(string path)
    {
        await using var stream = _azure.ReadBlob(Container, path);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }

    public Task<bool> ExistsAsync(string path)
        => Task.FromResult(_azure.Exists(Container, path));
}