using CloudStorageDemo.Models;

namespace CloudStorageDemo.Patterns.Proxy;

public interface IStorageService
{
    Task<byte[]> DownloadAsync(User user, string path);
    Task UploadAsync(User user, string path, byte[] data);
    void Invalidate(string path);
    IReadOnlyCollection<string> CachedKeys { get; }
}