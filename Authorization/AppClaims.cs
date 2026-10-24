namespace IpDeputyApi.Authorization;

public static class AppClaims
{
    public const string Subject = "sub";
    public const string Name = "name";
    public const string Kind = "kind";
    public const string StudentId = "student_id";
    public const string Permission = "perm";
    public const string TokenVersion = "tv";
}

public static class UserKinds
{
    public const string Admin = "admin";
    public const string Student = "student";
}
