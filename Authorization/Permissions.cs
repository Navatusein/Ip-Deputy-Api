namespace IpDeputyApi.Authorization;

public record PermissionDefinition(string Key, string Name);

public static class Permissions
{
    public const string ManageStudents = "manage-students";
    public const string ManageTeachers = "manage-teachers";
    public const string ManageSubjects = "manage-subjects";
    public const string ManageSchedule = "manage-schedule";
    public const string ManageSubmissions = "manage-submissions";
    public const string ManageLinks = "manage-links";
    public const string ManageUsers = "manage-users";

    public static readonly IReadOnlyList<PermissionDefinition> All =
    [
        new(ManageStudents, "Студенти"),
        new(ManageTeachers, "Викладачі"),
        new(ManageSubjects, "Предмети"),
        new(ManageSchedule, "Розклад"),
        new(ManageSubmissions, "Захист робіт"),
        new(ManageLinks, "Посилання"),
        new(ManageUsers, "Користувачі")
    ];

    public static IReadOnlyList<string> AllKeys { get; } = All.Select(x => x.Key).ToList();

    public static bool IsKnown(string key)
    {
        return AllKeys.Contains(key);
    }
}
