namespace eMechanic.Application.UserRepairPreferences.Features.Get;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.UserRepairPreferences;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

internal sealed class GetCurrentUserRepairPreferencesHandler
    : IResultQueryHandler<GetCurrentUserRepairPreferencesQuery, UserRepairPreferencesResponse>
{
    private readonly IUserRepairPreferencesRepository _preferencesRepository;
    private readonly IUserContext _userContext;

    public GetCurrentUserRepairPreferencesHandler(
        IUserRepairPreferencesRepository preferencesRepository,
        IUserContext userContext)
    {
        _preferencesRepository = preferencesRepository;
        _userContext = userContext;
    }

    public async Task<Result<UserRepairPreferencesResponse, Error>> Handle(
        GetCurrentUserRepairPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();

        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is null)
        {
            return new Error(EErrorCode.NotFoundError);
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
