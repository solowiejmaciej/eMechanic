// src/eMechanic.Backend/Core/src/eMechanic.Infrastructure/DAL/Configurations/VehicleConfiguration.cs
namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.Vehicle;
using Domain.Vehicle.Vehicle;
using Domain.Vehicle.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.OwnsOne(v => v.Vin, b =>
        {
            b.Property(vi => vi.Value)
                .HasColumnName("Vin")
                .HasMaxLength(17)
                .IsRequired();
        });

        builder.OwnsOne(v => v.Manufacturer, b =>
        {
            b.Property(m => m.Value)
                .HasColumnName("Manufacturer")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(v => v.Model, b =>
        {
            b.Property(m => m.Value)
                .HasColumnName("Model")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.OwnsOne(v => v.ProductionYear, b =>
        {
            b.Property(py => py.Value)
                .HasColumnName("ProductionYear")
                .HasMaxLength(4)
                .IsRequired();
        });

        builder.Property(v => v.EngineCapacity)
            .HasConversion(ec => ec!.Value,
                value => EngineCapacity.Create(value).Value!)
            .HasColumnType("decimal(4, 1)")
            .IsRequired(false);

        builder.OwnsOne(v => v.Mileage, mileageBuilder =>
        {
            mileageBuilder.Property(m => m.Value)
                .IsRequired();

            mileageBuilder.Property(m => m.Unit)
                .HasColumnName("MileageUnit")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(v => v.HorsePower)
            .HasConversion(py => py.Value, value => HorsePower.Create(value).Value!)
            .IsRequired();

        builder.OwnsOne(v => v.LicensePlate, b =>
        {
            b.Property(lp => lp.Value)
                .HasColumnName("LicensePlate")
                .HasMaxLength(15)
                .IsRequired();
        });

        builder.Property(v => v.FuelType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.BodyType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.VehicleType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.UpdatedAt);

        builder.HasIndex(x => x.UserId);
    }
}
