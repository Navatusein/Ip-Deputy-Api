using IpDeputyApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpDeputyApi.Database.Configurations;

public class CoupleConfiguration : IEntityTypeConfiguration<Couple>
{
    public void Configure(EntityTypeBuilder<Couple> builder)
    {
        builder.HasOne(x => x.Subject).WithMany(x => x.Couples).HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SubjectType).WithMany(x => x.Couples).HasForeignKey(x => x.SubjectTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DayOfWeek).WithMany(x => x.Couples).HasForeignKey(x => x.DayOfWeekId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CoupleTime).WithMany(x => x.Couples).HasForeignKey(x => x.CoupleTimeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Teacher).WithMany(x => x.Couples).HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Restrict);
    }
}
