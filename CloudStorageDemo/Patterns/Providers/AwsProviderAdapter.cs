namespace CloudStorageDemo.Patterns.Providers;
public sealed class AwsProviderAdapter : ICloudProvider
{
    private const string Bucket = "cloud-storage";
    private readonly AwsLikeProvider _aws;

    public AwsProviderAdapter(AwsLikeProvider aws)
        => _aws = aws ?? throw new ArgumentNullException(nameof(aws));

    public string Name => "AWS-like";

    public Task UploadAsync(string path, byte[] data)
    {
        _aws.PutObject(Bucket, path, data);
        return Task.CompletedTask;
    }

    public Task<byte[]> DownloadAsync(string path)
        => Task.FromResult(_aws.GetObject(Bucket, path));

    public Task<bool> ExistsAsync(string path)
        => Task.FromResult(_aws.HeadObject(Bucket, path));
}