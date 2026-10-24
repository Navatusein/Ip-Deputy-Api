using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Dto.Frontend;

public class PreferredPositionDto
{
    public int SubmissionsConfigId { get; set; }
    public PreferredPosition PreferredPosition { get; set; }
}
