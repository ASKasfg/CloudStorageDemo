namespace CloudStorageDemo.Models;

public sealed record AccessResult(bool Allowed, string? Reason = null)
{
    public static AccessResult Ok() => new(true);
    public static AccessResult Deny(string reason) => new(false, reason);
}