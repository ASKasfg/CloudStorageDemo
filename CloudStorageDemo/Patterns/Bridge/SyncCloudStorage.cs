using CloudStorageDemo.Patterns.Providers;

namespace CloudStorageDemo.Patterns.Bridge;

public sealed class SyncCloudStorage : CloudStorage
{
    public SyncCloudStorage(ICloudProvider provider) : base(provider) { }

    public override async Task UploadAsync(string path, byte[] data)
    {
        var p = NormalizePath(path);
        Console.WriteLine($"[Bridge/Sync] Загрузка '{p}' через {ProviderName} ({data.Length} байт)");
        await Provider.UploadAsync(p, data);
        Console.WriteLine($"[Bridge/Sync] OK — '{p}' сохранён в {ProviderName}");
    }

    public override async Task<byte[]> DownloadAsync(string path)
    {
        var p = NormalizePath(path);
        Console.WriteLine($"[Bridge/Sync] Скачивание '{p}' через {ProviderName}");
        var data = await Provider.DownloadAsync(p);
        Console.WriteLine($"[Bridge/Sync] OK — получено {data.Length} байт из {ProviderName}");
        return data;
    }

    public override Task<bool> ExistsAsync(string path)
        => Provider.ExistsAsync(NormalizePath(path));

    private static string NormalizePath(string path)
        => path.TrimStart('/').Replace('\\', '/');
}