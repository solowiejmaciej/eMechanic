using eMechanic.NotificationService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace eMechanic.NotificationService.DAL;


public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<NotificationUser> Users { get; set;}
    public DbSet<NotificationWorkshop> Workshops { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationUser>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.PhoneNumber);
        });

        modelBuilder.Entity<NotificationWorkshop>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.PhoneNumber);
        });

        //indeksacja na maila żeby szybciej wyszukać
        modelBuilder.Entity<NotificationUser>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<NotificationWorkshop>().HasIndex(u => u.Email).IsUnique();
    }
}
