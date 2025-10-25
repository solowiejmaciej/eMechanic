// src/eMechanic.Backend/Core/src/eMechanic.Infrastructure/DAL/Configurations/VehicleConfiguration.cs
namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.Vehicle;
using Domain.Vehicle.ValueObjects; // Upewnij się, że są usingi dla Value Objects
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

        builder.Property(v => v.Vin)
            .HasConversion(vin => vin.Value, value => Vin.Create(value).Value!)
            .HasMaxLength(17)
            .IsRequired();

        builder.Property(v => v.Manufacturer)
            .HasConversion(m => m.Value, value => Manufacturer.Create(value).Value!)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Model)
            .HasConversion(m => m.Value, value => Model.Create(value).Value!)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.ProductionYear)
            .HasConversion(py => py.Value, value => ProductionYear.Create(value).Value!)
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(v => v.EngineCapacity)
            .HasConversion(ec => ec!.Value,
                value => EngineCapacity.Create(value).Value!)
            .HasColumnType("decimal(4, 1)")
            .IsRequired(false);

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
