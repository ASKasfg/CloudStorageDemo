namespace CloudStorageDemo.Models;

public enum Role
{
    Guest,
    User,
    Admin
}

public sealed record User(string Name, Role Role)
{
    public override string ToString() => $"{Name} ({Role})";
}