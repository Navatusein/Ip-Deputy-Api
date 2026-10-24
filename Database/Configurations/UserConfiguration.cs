using IpDeputyApi.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IpDeputyApi.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Login).HasMaxLength(64);
        builder.Property(x => x.DisplayName).HasMaxLength(256);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.TokenVersion).HasDefaultValue(0);
        builder.HasIndex(x => x.Login).IsUnique();

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
