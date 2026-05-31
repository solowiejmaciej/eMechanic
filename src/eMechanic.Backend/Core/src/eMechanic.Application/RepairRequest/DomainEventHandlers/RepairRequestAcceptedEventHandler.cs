namespace eMechanic.Application.RepairRequest.DomainEventHandlers;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.DomainEvents;
using eMechanic.Application.Abstractions.Outbox;
using eMechanic.Application.Repair.Repositories;
using eMechanic.Application.Users.Repositories;
using eMechanic.Domain.RepairRequest.DomainEvents;
using eMechanic.Events.Events.RepairRequest;
using Vehicle.Vehicle.Repostories;

public class RepairRequestAcceptedEventHandler : IDomainEventHandler<RepairRequestAcceptedDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRepairRepository _repairRepository;
    private readonly IOutboxWriter _outboxWriter;

    public RepairRequestAcceptedEventHandler(
        IUserRepository userRepository,
        IVehicleRepository vehicleRepository,
        IRepairRepository repairRepository,
        IOutboxWriter outboxWriter)
    {
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _repairRepository = repairRepository;
        _outboxWriter = outboxWriter;
    }

    public async Task Handle(RepairRequestAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repairRequest = notification.RepairRequest;

        if (repairRequest.EstimatedCost is null)
        {
            return;
        }

        var createRepairResult = Domain.Repair.Repair.Create(
            repairRequest.VehicleId,
            repairRequest.WorkshopId,
            repairRequest.EstimatedCost!,
            repairRequest.Id);

        if (createRepairResult.HasError())
        {
            return;
        }

        await _repairRepository.AddAsync(createRepairResult.Value!, cancellationToken);

        var user = await _userRepository.GetByIdAsync(repairRequest.UserId, cancellationToken);
        var vehicle = await _vehicleRepository.GetByIdAsync(repairRequest.VehicleId, cancellationToken);

        if (user is null || vehicle is null)
        {
            return;
        }

        var integrationEvent = new RepairRequestAcceptedEvent(
            repairRequest.Id,
            user.Id,
            user.Email,
            "123-456-7890",
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
