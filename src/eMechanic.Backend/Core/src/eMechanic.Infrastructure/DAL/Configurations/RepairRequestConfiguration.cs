namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.RepairRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RepairRequestConfiguration : IEntityTypeConfiguration<RepairRequest>
{
    public void Configure(EntityTypeBuilder<RepairRequest> builder)
    {
        builder.ToTable("RepairRequests");

        builder.HasKey(rr => rr.Id);

        builder.Property(rr => rr.UserId).IsRequired();
        builder.Property(rr => rr.WorkshopId).IsRequired();
        builder.Property(rr => rr.VehicleId).IsRequired();

        builder.OwnsOne(rr => rr.Description, b =>
        {
            b.Property(d => d.Value)
                .HasColumnName("Description")
                .HasMaxLength(2000)
                .IsRequired();
        });

        builder.OwnsOne(rr => rr.Diagnosis, b =>
        {
            b.Property(d => d.Value)
                .HasColumnName("Diagnosis")
                .HasMaxLength(4000)
                .IsRequired(false);
        });
        builder.Navigation(rr => rr.Diagnosis).IsRequired(false);

        builder.OwnsOne(rr => rr.EstimatedCost, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("EstimatedCostAmount")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("EstimatedCostCurrency")
                .HasMaxLength(3);
        });

        builder.Property(rr => rr.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(rr => rr.RejectionReason, b =>
        {
            b.Property(r => r.Value)
                .HasColumnName("RejectionReason")
                .HasMaxLength(500)
                .IsRequired(false);
        });
        builder.Navigation(rr => rr.RejectionReason).IsRequired(false);

        builder.OwnsOne(rr => rr.SummaryReport, b =>
        {
            b.Property(s => s.Value)
                .HasColumnName("SummaryReport")
                .HasMaxLength(4000)
                .IsRequired(false);
        });
        builder.Navigation(rr => rr.SummaryReport).IsRequired(false);

        builder.Property(rr => rr.CreatedAt).IsRequired();
        builder.Property(rr => rr.UpdatedAt);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.WorkshopId);
        builder.HasIndex(x => x.VehicleId);
    }
}
