namespace AbySalto.Junior.Domain.Enums;

public static class UserRoleNames
{
    public static string From(UserRole role) => role.ToString();

    public static IEnumerable<string> All() => Enum.GetNames<UserRole>();
}
