namespace eMechanic.Infrastructure.DAL.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Outbox;

internal sealed class OutboxMessagesConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Payload).IsRequired();
        builder.HasIndex(x => x.ProcessedAt)
            .HasFilter("\"ProcessedAt\" IS NULL");
    }
}
