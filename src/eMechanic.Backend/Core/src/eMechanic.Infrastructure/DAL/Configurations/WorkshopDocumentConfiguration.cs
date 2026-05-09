namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.Workshop.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class WorkshopDocumentConfiguration : IEntityTypeConfiguration<WorkshopDocument>
{
    public void Configure(EntityTypeBuilder<WorkshopDocument> builder)
    {
        builder.ToTable("WorkshopDocuments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkshopId).IsRequired();
        builder.Property(x => x.FullPath).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();

        builder.Property(x => x.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.WorkshopId);
    }
}
