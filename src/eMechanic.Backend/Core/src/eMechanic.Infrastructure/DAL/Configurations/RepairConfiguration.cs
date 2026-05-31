namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.Repair;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RepairConfiguration : IEntityTypeConfiguration<Repair>
{
    public void Configure(EntityTypeBuilder<Repair> builder)
    {
        builder.ToTable("Repairs");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RepairRequestId)
            .IsRequired(false);

        builder.Property(r => r.VehicleId)
            .IsRequired();

        builder.Property(r => r.WorkshopId)
            .IsRequired();

        builder.OwnsOne(r => r.EstimatedCost, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("EstimatedCostAmount")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("EstimatedCostCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(r => r.FinalCost, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("FinalCostAmount")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("FinalCostCurrency")
                .HasMaxLength(3);
        });

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt);

        builder.HasIndex(r => r.RepairRequestId).IsUnique();
        builder.HasIndex(r => r.VehicleId);
        builder.HasIndex(r => r.WorkshopId);
    }
}

