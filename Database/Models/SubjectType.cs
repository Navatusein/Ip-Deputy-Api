namespace IpDeputyApi.Database.Models
{
    public enum SubjectTypes
    {
        Laboratory = 1,
        Practice = 2,
        Lecture = 3,
        Seminar = 4,
        Consultation = 5,
        Credit = 6,
        Exam = 7,
        Extra = 8
    }

    public class SubjectType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;

        public virtual IEnumerable<Couple> Couples { get; } = new List<Couple>();
        public virtual IEnumerable<AdditionalCouple> AdditionalCouples { get; } = new List<AdditionalCouple>();
        public virtual IEnumerable<SubmissionsConfig> SubmissionsConfigs { get; } = new List<SubmissionsConfig>();
    }
}
