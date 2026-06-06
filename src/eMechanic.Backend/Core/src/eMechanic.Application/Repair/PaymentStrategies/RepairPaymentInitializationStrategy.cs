namespace eMechanic.Application.Repair.PaymentStrategies;

using Domain.Payment.Enums;
using eMechanic.Application.Payments.Common;
using eMechanic.Application.Payments.Strategies;
using eMechanic.Application.Repair.Repositories;
using eMechanic.Application.Vehicle.Vehicle.Services;
using eMechanic.Common.Result;
using eMechanic.Domain.Repair.Enums;

public sealed class RepairPaymentInitializationStrategy : IPaymentInitializationStrategy
{
    private readonly IRepairRepository _repairRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;

    public EPayableType SupportedType => EPayableType.Repair;

    public RepairPaymentInitializationStrategy(
        IRepairRepository repairRepository,
        IVehicleOwnershipService vehicleOwnershipService)
    {
        _repairRepository = repairRepository;
        _vehicleOwnershipService = vehicleOwnershipService;
    }

    public async Task<Result<PayableItem, Error>> BuildPayableItemAsync(
        Guid referenceId,
        CancellationToken cancellationToken)
    {
        var repair = await _repairRepository.GetByIdAsync(referenceId, cancellationToken);

        if (repair is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair with ID {referenceId} not found.");
        }

        if (repair.Status != ERepairStatus.Completed)
        {
            return new Error(
                EErrorCode.ValidationError,
                $"Repair must be in '{ERepairStatus.Completed}' status to initialize payment. Current status: '{repair.Status}'.");
        }

        if (repair.FinalCost is null)
        {
            return new Error(EErrorCode.ValidationError, "Repair does not have a final cost set.");
        }

        var ownershipResult = await _vehicleOwnershipService.GetAndVerifyOwnershipAsync(
            repair.VehicleId, cancellationToken);

        if (ownershipResult.HasError())
        {
            return ownershipResult.Error!;
        }

        return new PayableItem(repair.Id, EPayableType.Repair, repair.FinalCost, ownershipResult.Value!.UserId);
    }
}
