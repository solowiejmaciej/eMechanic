namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.VehicleTimeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VehicleTimelineConfiguration
{
    public void Configure(EntityTypeBuilder<VehicleTimeline> builder)
    {
        builder.ToTable("VehicleTimeline");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.VehicleId).IsRequired();

        builder.Property(t => t.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Data)
            .IsRequired()
            .HasMaxLength(2048);

        builder.HasIndex(t => t.VehicleId);
    }
}
