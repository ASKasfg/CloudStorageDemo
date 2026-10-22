using CloudStorageDemo.Models;
using CloudStorageDemo.Patterns.Bridge;
using CloudStorageDemo.Patterns.Proxy;

namespace CloudStorageDemo.Patterns.Facade;

public sealed class SyncService
{
    private readonly CloudStorage _storage;
    private readonly IStorageService _service;

    public SyncService(CloudStorage storage, IStorageService service)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public string ProviderName => _storage.ProviderName;

    public Task UploadAsync(User user, string path, string text)
        => _service.UploadAsync(user, path, System.Text.Encoding.UTF8.GetBytes(text));

    public async Task<string> DownloadAsync(User user, string path)
    {
        var bytes = await _service.DownloadAsync(user, path);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}