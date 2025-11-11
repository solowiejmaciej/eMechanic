namespace eMechanic.Infrastructure.DAL.Configurations;

using eMechanic.Domain.Workshop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Workshops");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(w => w.ContactEmail)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(w => w.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Address)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(w => w.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.IdentityId)
            .IsRequired();

        builder.HasIndex(w => w.Email).IsUnique();

        builder.HasIndex(w => w.IdentityId).IsUnique();
    }
}
