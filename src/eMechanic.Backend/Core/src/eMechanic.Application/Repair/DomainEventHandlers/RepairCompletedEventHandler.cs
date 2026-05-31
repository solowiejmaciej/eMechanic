namespace eMechanic.Application.Repair.DomainEventHandlers;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.Outbox;
using eMechanic.Application.Users.Repositories;
using eMechanic.Domain.Repair.DomainEvents;
using eMechanic.Events.Events.Repair;
using Vehicle.Vehicle.Repostories;

public class RepairCompletedEventHandler : IDomainEventHandler<RepairCompletedDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IOutboxWriter _outboxWriter;

    public RepairCompletedEventHandler(
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        IOutboxWriter outboxWriter)
    {
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _outboxWriter = outboxWriter;
    }

    public async Task Handle(RepairCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repair = notification.Repair;

        if (repair.FinalCost is null)
        {
            return;
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(repair.VehicleId, cancellationToken);
        if (vehicle is null)
        {
            return;
        }

        var user = await _userRepository.GetByIdAsync(vehicle.UserId, cancellationToken);
        if (user is null)
        {
            return;
        }

        var integrationEvent = new RepairCompletedEvent(
            repair.Id,
            repair.RepairRequestId,
            user.Id,
            user.Email,
            "123-456-7890",
            user.FirstName,
            vehicle.Id,
            vehicle.Vin.Value,
            vehicle.Model.Value,
            vehicle.LicensePlate.Value,
            vehicle.ProductionYear.Value,
            vehicle.Manufacturer.Value,
            repair.EstimatedCost.Amount,
            repair.EstimatedCost.Currency,
            repair.FinalCost.Amount,
            repair.FinalCost.Currency
        );

        await _outboxWriter.WriteAsync(integrationEvent, cancellationToken);
    }
}

