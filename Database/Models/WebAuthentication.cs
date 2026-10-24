namespace IpDeputyApi.Database.Models;

public class WebAuthentication : IEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public Student Student { get; set; } = null!;
}
