using System.ComponentModel.DataAnnotations;

namespace IpDeputyApi.Options;

public class DatabaseOptions
{
    public const string Section = "Database";

    [Required]
    public string ConnectionString { get; set; } = "";

    public bool ApplyMigrationsOnStartup { get; set; } = true;
}
