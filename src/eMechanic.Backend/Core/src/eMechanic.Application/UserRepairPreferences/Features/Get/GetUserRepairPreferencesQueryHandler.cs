namespace eMechanic.Application.UserRepairPreferences.Features.Get;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.RepairRequest.Repositories;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Repositories;

internal sealed class GetUserRepairPreferencesQueryHandler
    : IResultQueryHandler<GetUserRepairPreferencesQuery, UserRepairPreferencesResponse>
{
    private readonly IUserRepairPreferencesRepository _preferencesRepository;
    private readonly IWorkshopContext _workshopContext;
    private readonly IRepairRequestRepository _repairRequestRepository;

    public GetUserRepairPreferencesQueryHandler(
        IUserRepairPreferencesRepository preferencesRepository,
        IWorkshopContext workshopContext,
        IRepairRequestRepository repairRequestRepository)
    {
        _preferencesRepository = preferencesRepository;
        _workshopContext = workshopContext;
        _repairRequestRepository = repairRequestRepository;
    }

    public async Task<Result<UserRepairPreferencesResponse, Error>> Handle(
        GetUserRepairPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        var workshopId = _workshopContext.GetWorkshopId();

        var hasRelation = await _repairRequestRepository.HasRelationWithUserAsync(workshopId, request.UserId, cancellationToken);
        if (!hasRelation)
        {
            return new Error(EErrorCode.UnauthorizedError, "Workshop does not have any active requests or repairs with this user.");
        }

        var preferences = await _preferencesRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (preferences is null)
        {
            return new Error(EErrorCode.NotFoundError, "Preferences not found for this user.");
        }

        var response = new UserRepairPreferencesResponse(
            preferences.Id,
            preferences.UserId,
            preferences.PartsPreference,
            preferences.TimelinePreference
        );

        return response;
    }
}
