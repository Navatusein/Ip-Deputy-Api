using IpDeputyApi.Database.Models;

namespace IpDeputyApi.Dto.Frontend;

public class MySubmissionCreateDto
{
    public int SubmissionsConfigId { get; set; }
    public List<int> SubmissionWorkIds { get; set; } = new();
    public PreferredPosition PreferredPosition { get; set; } = PreferredPosition.DoesNotMatter;
}
