namespace CloudStorageDemo.Patterns.Providers;
public sealed class AzureLikeProvider
{
    private readonly Dictionary<string, byte[]> _blobs = new();

    public void CreateBlob(string container, string blobName, Stream data)
    {
        using var ms = new MemoryStream();
        data.CopyTo(ms);
        _blobs[$"{container}::{blobName}"] = ms.ToArray();
    }

    public Stream ReadBlob(string container, string blobName)
    {
        var composite = $"{container}::{blobName}";
        if (!_blobs.TryGetValue(composite, out var data))
            throw new KeyNotFoundException($"Azure: блоб не найден {composite}");
        return new MemoryStream(data, writable: false);
    }

    public bool Exists(string container, string blobName)
        => _blobs.ContainsKey($"{container}::{blobName}");
}