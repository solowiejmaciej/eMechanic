namespace eMechanic.Infrastructure.DAL;

using Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class IdentityAppDbContext : IdentityDbContext<Identity, IdentityRole<Guid>, Guid>
{
    private const string IDENTITY_SCHEMA = "identity";

    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(IDENTITY_SCHEMA);

        base.OnModelCreating(builder);
    }
}
