namespace eMechanic.Application.Vehicle.Vehicle.Services;

using Domain.Vehicle.Vehicle;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.RepairRequest.Repositories;
using eMechanic.Application.Repair.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using Repositories;
using System.Linq;

public sealed class VehicleOwnershipService : IVehicleOwnershipService
{
    private readonly IUserContext _userContext;
    private readonly IWorkshopContext _workshopContext;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IRepairRepository _repairRepository;

    public VehicleOwnershipService(
        IUserContext userContext,
        IWorkshopContext workshopContext,
        IVehicleRepository vehicleRepository,
        IRepairRequestRepository repairRequestRepository,
        IRepairRepository repairRepository)
    {
        _userContext = userContext;
        _workshopContext = workshopContext;
        _vehicleRepository = vehicleRepository;
        _repairRequestRepository = repairRequestRepository;
        _repairRepository = repairRepository;
    }

    public async Task<Result<Vehicle, Error>> GetAndVerifyOwnershipAsync(
        Guid vehicleId,
        CancellationToken cancellationToken)
    {
        if (_workshopContext.IsAuthenticated)
        {
            try
            {
                var workshopId = _workshopContext.GetWorkshopId();
                var requests = await _repairRequestRepository.GetForWorkshopAsync(
                    workshopId,
                    new PaginationParameters { PageNumber = 1, PageSize = 100 },
                    cancellationToken);
                var repairs = await _repairRepository.GetForWorkshopPaginatedAsync(
                    workshopId,
                    new PaginationParameters { PageNumber = 1, PageSize = 100 },
                    cancellationToken);

                bool hasAssociation = requests.Items.Any(r => r.VehicleId == vehicleId) ||
                                      repairs.Items.Any(r => r.VehicleId == vehicleId);

                if (!hasAssociation)
                {
                    return new Error(EErrorCode.UnauthorizedError, "Workshop is not associated with this vehicle.");
                }

                var workshopVehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
                if (workshopVehicle is null)
                {
                    return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{vehicleId}' not found.");
                }
                return workshopVehicle;
            }
            catch (UnauthorizedAccessException)
            {
                // Fall back to user check if workshop claim is invalid
            }
        }

        Guid currentUserId;
        try
        {
            currentUserId = _userContext.GetUserId();
        }
        catch (UnauthorizedAccessException ex)
        {
            return new Error(EErrorCode.UnauthorizedError, ex.Message);
        }

        var vehicle = await _vehicleRepository.GetForUserById(
            vehicleId,
            currentUserId,
            cancellationToken);

        if (vehicle is null)
        {
            return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{vehicleId}' not found.");
        }

        return vehicle;
    }

    public async Task<Result<Success, Error>> VerifyOwnershipAsync(Guid vehicleId, CancellationToken cancellationToken)
    {
        if (_workshopContext.IsAuthenticated)
        {
            try
            {
                var workshopId = _workshopContext.GetWorkshopId();
                var requests = await _repairRequestRepository.GetForWorkshopAsync(
                    workshopId,
                    new PaginationParameters { PageNumber = 1, PageSize = 100 },
                    cancellationToken);
                var repairs = await _repairRepository.GetForWorkshopPaginatedAsync(
                    workshopId,
                    new PaginationParameters { PageNumber = 1, PageSize = 100 },
                    cancellationToken);

                bool hasAssociation = requests.Items.Any(r => r.VehicleId == vehicleId) ||
                                      repairs.Items.Any(r => r.VehicleId == vehicleId);

                if (!hasAssociation)
                {
                    return new Error(EErrorCode.UnauthorizedError, "Workshop is not associated with this vehicle.");
                }

                var workshopVehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
                if (workshopVehicle is not null)
                {
                    return Result.Success;
                }
                return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{vehicleId}' not found.");
            }
            catch (UnauthorizedAccessException)
            {
                // Fall back to user check
            }
        }

        Guid currentUserId;
        try
        {
            currentUserId = _userContext.GetUserId();
        }
        catch (UnauthorizedAccessException ex)
        {
            return new Error(EErrorCode.UnauthorizedError, ex.Message);
        }

        var result = await _vehicleRepository.ExistsForUserAsync(
            vehicleId,
            currentUserId,
            cancellationToken);

        if (!result)
        {
            return new Error(EErrorCode.NotFoundError, $"Vehicle with Id '{vehicleId}' not found.");
        }

        return Result.Success;
    }
}
