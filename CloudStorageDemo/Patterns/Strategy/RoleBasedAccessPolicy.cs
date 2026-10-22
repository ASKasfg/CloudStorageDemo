using CloudStorageDemo.Models;

namespace CloudStorageDemo.Patterns.Strategy;

public sealed class RoleBasedAccessPolicy : IAccessPolicy
{
    public AccessResult Check(User user, string path, Operation operation)
    {
        if (user is null) return AccessResult.Deny("Пользователь не задан");
        if (string.IsNullOrWhiteSpace(path)) return AccessResult.Deny("Пустой путь");

        var isPrivate = path.StartsWith("private/", StringComparison.OrdinalIgnoreCase);

        if (user.Role == Role.Admin)
            return AccessResult.Ok();

        if (operation == Operation.Read)
        {
            if (isPrivate && user.Role == Role.Guest)
                return AccessResult.Deny("Guest не имеет доступа к private/*");
            return AccessResult.Ok();
        }

        if (user.Role == Role.Guest)
            return AccessResult.Deny("Guest не может писать");
        if (isPrivate && user.Role == Role.User)
            return AccessResult.Deny("User не может писать в private/*");

        return AccessResult.Ok();
    }
}