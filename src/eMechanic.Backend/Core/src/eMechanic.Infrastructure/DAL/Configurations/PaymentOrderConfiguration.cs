namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PaymentOrderConfiguration : IEntityTypeConfiguration<PaymentOrder>
{
    public void Configure(EntityTypeBuilder<PaymentOrder> builder)
    {
        builder.ToTable("PaymentOrders");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProviderSessionId)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(p => p.ProviderSessionId)
            .IsUnique();

        builder.Property(p => p.CheckoutUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(p => p.ReferenceId)
            .IsRequired();

        builder.Property(p => p.PayableType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(p => p.Amount, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasColumnName("AmountValue")
                .HasColumnType("decimal(18,2)");

            moneyBuilder.Property(m => m.Currency)
                .HasColumnName("AmountCurrency")
                .HasMaxLength(3);
        });

        builder.Property(p => p.PayerId)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt);

        builder.HasIndex(p => new { p.ReferenceId, p.PayableType, p.Status });
    }
}
