namespace IpDeputyApi.Database.Models;

public class Subgroup : IEntity
{
    public int Id { get; set; }
    public int Index { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<SubmissionsConfig> SubmissionsConfigs { get; set; } = new List<SubmissionsConfig>();
}
