namespace IpDeputyApi.Database.Models;

public class User : IEntity
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int? StudentId { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? LegacyPasswordSalt { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public int TokenVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public Student? Student { get; set; }
}
