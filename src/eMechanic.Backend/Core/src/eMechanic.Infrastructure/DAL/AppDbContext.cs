namespace eMechanic.Infrastructure.DAL;

using System.Reflection;
using System.Text.Json;
using Common.DDD;
using Domain.User;
using Domain.Vehicle;
using Domain.VehicleTimeline;
using Domain.Workshop;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Outbox;

public class AppDbContext : DbContext
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleTimeline> VehicleTimelines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync();
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var eventsToPublish = ChangeTracker.Entries<AggregateRoot>()
            .Select(aggregateRoot => aggregateRoot.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            }).ToList();

        var outboxEvents = eventsToPublish
            .OfType<IOutboxMessage>()
            .ToList();

        foreach (var outboxEvent in outboxEvents)
        {
            var outboxMessage = new OutboxMessage(
                outboxEvent.GetType().Name,
                JsonSerializer.Serialize(outboxEvent, outboxEvent.GetType())
            );

            await OutboxMessages.AddAsync(outboxMessage);
        }

        var tasks = eventsToPublish
            .Select(async domainEvent => { await _mediator.Publish(domainEvent); });

        await Task.WhenAll(tasks);
    }
}
