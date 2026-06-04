
namespace eMechanic.Application.RepairRequest.DomainEventHandlers;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.Outbox;
using eMechanic.Application.Users.Repositories;
using eMechanic.Domain.RepairRequest.DomainEvents;
using eMechanic.Events.Events.RepairRequest;
using eMechanic.Application.Vehicle.Vehicle.Repositories;

public class RepairRequestCreatedEventHandler : IDomainEventHandler<RepairRequestCreatedDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IOutboxWriter _outboxWriter;

    public RepairRequestCreatedEventHandler(
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        IOutboxWriter outboxWriter)
    {
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _outboxWriter = outboxWriter;
    }

    public async Task Handle(RepairRequestCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repairRequest = notification.RepairRequest;

        var user = await _userRepository.GetByIdAsync(repairRequest.UserId, cancellationToken);
        var vehicle = await _vehicleRepository.GetByIdAsync(repairRequest.VehicleId, cancellationToken);

        if (user is null || vehicle is null)
        {
            return;
        }

        var integrationEvent = new RepairRequestCreatedEvent(
            repairRequest.Id,
            user.Id,
            user.Email.Value,
            user.PhoneNumber?.Value ?? string.Empty,
            user.FirstName,
            vehicle.Id,
            vehicle.Vin.Value,
            vehicle.Model.Value,
            vehicle.LicensePlate.Value,
            vehicle.ProductionYear.Value,
            vehicle.Manufacturer.Value
        );

        await _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
    }
}
