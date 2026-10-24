namespace IpDeputyApi.Database.Models;

public class Teacher : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public string? ContactPhone { get; set; }
    public string? Email { get; set; }
    public string? FitEmail { get; set; }
    public string? TelegramNickname { get; set; }

    public ICollection<Couple> Couples { get; set; } = new List<Couple>();
    public ICollection<AdditionalCouple> AdditionalCouples { get; set; } = new List<AdditionalCouple>();
}
