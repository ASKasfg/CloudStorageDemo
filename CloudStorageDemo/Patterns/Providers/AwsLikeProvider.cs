namespace CloudStorageDemo.Patterns.Providers;

public sealed class AwsLikeProvider
{
    private readonly Dictionary<string, byte[]> _objects = new();

    public void PutObject(string bucket, string key, byte[] payload)
        => _objects[$"{bucket}/{key}"] = payload;

    public byte[] GetObject(string bucket, string key)
    {
        var composite = $"{bucket}/{key}";
        if (!_objects.TryGetValue(composite, out var data))
            throw new FileNotFoundException($"AWS: объект не найден {composite}");
        return data;
    }

    public bool HeadObject(string bucket, string key)
        => _objects.ContainsKey($"{bucket}/{key}");
}