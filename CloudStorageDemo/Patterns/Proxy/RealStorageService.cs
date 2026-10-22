using CloudStorageDemo.Models;
using CloudStorageDemo.Patterns.Bridge;

namespace CloudStorageDemo.Patterns.Proxy;

public sealed class RealStorageService : IStorageService
{
    private readonly CloudStorage _storage;

    public RealStorageService(CloudStorage storage)
        => _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    public Task<byte[]> DownloadAsync(User user, string path) => _storage.DownloadAsync(path);
    public Task UploadAsync(User user, string path, byte[] data) => _storage.UploadAsync(path, data);
    public void Invalidate(string path) { }
    public IReadOnlyCollection<string> CachedKeys => Array.Empty<string>();
}