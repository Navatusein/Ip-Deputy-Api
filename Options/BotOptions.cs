using System.ComponentModel.DataAnnotations;

namespace IpDeputyApi.Options;

public class BotOptions
{
    [Required]
    public string BotAuthorizeToken { get; set; } = "";

    [Required]
    public string BotToken { get; set; } = "";

    [Range(1, 168)]
    public int InitDataLifetimeHours { get; set; } = 24;
}
