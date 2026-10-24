namespace IpDeputyApi.Database.Models;

public class DayOfWeek : IEntity
{
    public int Id { get; set; }
    public int Index { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Couple> Couples { get; set; } = new List<Couple>();
    public ICollection<AdditionalCouple> AdditionalCouples { get; set; } = new List<AdditionalCouple>();
}
