namespace eMechanic.Application.UserRepairPreferences.Features.Update;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using Repositories;

internal sealed class UpdateUserRepairPreferencesHandler
    : IResultCommandHandler<UpdateUserRepairPreferencesCommand, Success>
{
    private readonly IUserRepairPreferencesRepository _preferencesRepository;
    private readonly IUserContext _userContext;

    public UpdateUserRepairPreferencesHandler(
        IUserRepairPreferencesRepository preferencesRepository,
        IUserContext userContext)
    {
        _preferencesRepository = preferencesRepository;
        _userContext = userContext;
    }

    public async Task<Result<Success, Error>> Handle(
        UpdateUserRepairPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.GetUserId();
        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is null)
        {
            return new Error(EErrorCode.NotFoundError, "Unable to find user repair preferences");
        }

        preferences.UpdatePartsPreference(request.PartsPreference);
        preferences.UpdateTimelinePreference(request.TimelinePreference);

        await _preferencesRepository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
