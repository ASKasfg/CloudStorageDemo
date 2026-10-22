using CloudStorageDemo.Models;

namespace CloudStorageDemo.Patterns.Strategy;

public enum Operation
{
    Read,
    Write
}

public interface IAccessPolicy
{
    AccessResult Check(User user, string path, Operation operation);
}