using IpDeputyApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpDeputyApi.Database.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.Property(x => x.LaboratoryCount).HasDefaultValue(0);
        builder.Property(x => x.PracticalCount).HasDefaultValue(0);
    }
}
