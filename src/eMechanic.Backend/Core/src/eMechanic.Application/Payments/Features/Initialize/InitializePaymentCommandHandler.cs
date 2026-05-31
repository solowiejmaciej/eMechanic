namespace eMechanic.Application.Payments.Features.Initialize;

using Common;
using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Repair.Repositories;
using eMechanic.Application.Vehicle.Vehicle.Services;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using eMechanic.Domain.Repair.Enums;

public sealed class InitializePaymentCommandHandler
    : IResultCommandHandler<InitializePaymentCommand, PaymentSessionDto>
{
    private readonly IRepairRepository _repairRepository;
    private readonly IVehicleOwnershipService _vehicleOwnershipService;
    private readonly IPaymentService _paymentService;

    public InitializePaymentCommandHandler(
        IRepairRepository repairRepository,
        IVehicleOwnershipService vehicleOwnershipService,
        IPaymentService paymentService)
    {
        _repairRepository = repairRepository;
        _vehicleOwnershipService = vehicleOwnershipService;
        _paymentService = paymentService;
    }

    public async Task<Result<PaymentSessionDto, Error>> Handle(
        InitializePaymentCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Type != EPayableType.Repair)
        {
            return new Error(EErrorCode.ValidationError, $"Unsupported payable type: '{request.Type}'.");
        }

        var repair = await _repairRepository.GetByIdAsync(request.ReferenceId, cancellationToken);

        if (repair is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Repair with ID {request.ReferenceId} not found.");
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

        var vehicle = ownershipResult.Value!;

        var payableItem = new PayableItem(
            repair.Id,
            EPayableType.Repair,
            repair.FinalCost,
            vehicle.UserId);

        return await _paymentService.CreateCheckoutSessionAsync(
            payableItem,
            request.SuccessUrl,
            request.CancelUrl,
            cancellationToken);
    }
}

