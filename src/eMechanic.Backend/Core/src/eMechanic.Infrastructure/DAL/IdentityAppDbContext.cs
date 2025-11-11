namespace eMechanic.Infrastructure.DAL;

using Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityAppDbContext : IdentityDbContext<Identity, IdentityRole<Guid>, Guid>
{
    private const string IDENTITY_SCHEMA = "identity";
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(IDENTITY_SCHEMA);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.Property(rt => rt.Token).IsRequired();

            entity.HasOne<Identity>()
                .WithMany()
                .HasForeignKey(rt => rt.IdentityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(builder);
    }
}
