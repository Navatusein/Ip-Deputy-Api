namespace IpDeputyApi.Database.Models;

public class Subject : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public int LaboratoryCount { get; set; }
    public int PracticalCount { get; set; }

    public ICollection<Couple> Couples { get; set; } = new List<Couple>();
    public ICollection<AdditionalCouple> AdditionalCouples { get; set; } = new List<AdditionalCouple>();
    public ICollection<SubmissionsConfig> SubmissionsConfigs { get; set; } = new List<SubmissionsConfig>();
}
