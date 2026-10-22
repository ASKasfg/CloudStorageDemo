namespace CloudStorageDemo.Models;

public sealed record CloudFile(string Name, byte[] Content)
{
    public int SizeBytes => Content.Length;

    public static CloudFile FromText(string name, string text)
        => new(name, System.Text.Encoding.UTF8.GetBytes(text));

    public string AsText() => System.Text.Encoding.UTF8.GetString(Content);
}