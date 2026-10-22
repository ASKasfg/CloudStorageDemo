namespace CloudStorageDemo.Patterns.Providers;

public interface ICloudProvider
{
    string Name { get; }
    Task UploadAsync(string path, byte[] data);
    Task<byte[]> DownloadAsync(string path);
    Task<bool> ExistsAsync(string path);
}