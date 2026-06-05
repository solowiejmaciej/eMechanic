namespace eMechanic.Infrastructure.DAL.Configurations;

using Domain.Workshop.Reviews;
using Domain.Workshop.Reviews.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class WorkshopReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("WorkshopReviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkshopId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasConversion(
                rating => rating.Value,
                dbValue => ReviewRating.Create(dbValue).Value)
            .IsRequired();

        builder.OwnsOne(x => x.Comment, commentBuilder =>
        {
            commentBuilder.Property(c => c.Value)
                .HasColumnName("Comment")
                .HasMaxLength(ReviewComment.MAX_LENGTH)
                .IsRequired(false);
        });

        builder.Navigation(x => x.Comment).IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.WorkshopId);
        builder.HasIndex(x => new { x.WorkshopId, x.UserId }).IsUnique();
    }
}
