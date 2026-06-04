namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.RepairRequest;
using Domain.RepairRequest.ValueObjects;
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

        builder.Property(rr => rr.Description)
            .HasConversion(
                d => d.Value,
                v => RepairDescription.Create(v).Value!)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(rr => rr.Diagnosis)
            .HasConversion(
                d => d != null ? d.Value : null,
                v => !string.IsNullOrEmpty(v) ? RepairDiagnosis.Create(v).Value! : null)
            .HasMaxLength(4000)
            .IsRequired(false);

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

        builder.Property(rr => rr.RejectionReason)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => !string.IsNullOrEmpty(v) ? RejectionReason.Create(v).Value! : null)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(rr => rr.SummaryReport)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => !string.IsNullOrEmpty(v) ? SummaryReport.Create(v).Value! : null)
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(rr => rr.CreatedAt).IsRequired();
        builder.Property(rr => rr.UpdatedAt);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.WorkshopId);
        builder.HasIndex(x => x.VehicleId);
    }
}
