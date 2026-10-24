using IpDeputyApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpDeputyApi.Database.Configurations;

public class AdditionalCoupleConfiguration : IEntityTypeConfiguration<AdditionalCouple>
{
    public void Configure(EntityTypeBuilder<AdditionalCouple> builder)
    {
        builder.HasOne(x => x.Subject).WithMany(x => x.AdditionalCouples).HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SubjectType).WithMany(x => x.AdditionalCouples).HasForeignKey(x => x.SubjectTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DayOfWeek).WithMany(x => x.AdditionalCouples).HasForeignKey(x => x.DayOfWeekId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Teacher).WithMany(x => x.AdditionalCouples).HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Restrict);
    }
}
