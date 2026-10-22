using CloudStorageDemo.Patterns.Providers;

namespace CloudStorageDemo.Patterns.Bridge;

public abstract class CloudStorage
{
    protected readonly ICloudProvider Provider;

    protected CloudStorage(ICloudProvider provider)
        => Provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public string ProviderName => Provider.Name;

    public abstract Task UploadAsync(string path, byte[] data);
    public abstract Task<byte[]> DownloadAsync(string path);
    public virtual Task<bool> ExistsAsync(string path) => Provider.ExistsAsync(path);
}