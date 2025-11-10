namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.UserRepairPreferences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserRepairPreferencesConfiguration : IEntityTypeConfiguration<UserRepairPreferences>
{
    public void Configure(EntityTypeBuilder<UserRepairPreferences> builder)
    {
        builder.ToTable("UserRepairPreferences");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.Property(p => p.PartsPreference)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.TimelinePreference)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.UpdatedAt);
    }
}
