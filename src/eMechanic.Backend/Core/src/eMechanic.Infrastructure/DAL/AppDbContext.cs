namespace eMechanic.Infrastructure.DAL;

using System.Reflection;
using System.Text.Json;
using Common.DDD;
using Domain.User;
using Domain.UserRepairPreferences;
using Domain.Vehicle;
using Domain.VehicleDocument;
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
    public DbSet<UserRepairPreferences> UserRepairPreferences { get; set; }
    public DbSet<VehicleDocument> VehicleDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = GetDomainEvents();

        await PrepareOutboxMessagesAsync(domainEvents, cancellationToken);
        //If this is before save tiemline works correctly, published events will not include Id of added entity
        //But if we put this after save the Timeline stops working because there is not save in the handlers
        await PublishInProcessEventsAsync(domainEvents, cancellationToken);

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker.Entries<AggregateRoot>()
            .Select(aggregateRoot => aggregateRoot.Entity)
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            }).ToList();
    }

    private async Task PrepareOutboxMessagesAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        var outboxEvents = domainEvents.OfType<IOutboxMessage>().ToList();

        foreach (var outboxEvent in outboxEvents)
        {
            var @event = outboxEvent.MapToEvent();

            var outboxMessage = new OutboxMessage(
                @event.GetType().Name,
                JsonSerializer.Serialize(@event, @event.GetType())
            );

            await OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }

    private async Task PublishInProcessEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        var tasks = domainEvents
            .Select(async domainEvent =>
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            });

        await Task.WhenAll(tasks);
    }
}
