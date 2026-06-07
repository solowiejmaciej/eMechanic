namespace eMechanic.Application.UserRepairPreferences.Features.Update;

using Common.Cache.Attributes;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using eMechanic.Domain.UserRepairPreferences.Enums;
using FluentValidation;
using Get;

[InvalidatesCache(typeof(GetCurrentUserRepairPreferencesQuery))]
public sealed record UpdateUserRepairPreferencesCommand(
    EPartsPreference PartsPreference,
    ETimelinePreference TimelinePreference) : IResultCommand<Success>;

public class UpdateUserRepairPreferencesCommandValidator : AbstractValidator<UpdateUserRepairPreferencesCommand>
{
    public UpdateUserRepairPreferencesCommandValidator()
    {
        RuleFor(x => x.PartsPreference)
            .IsInEnum()
            .WithMessage("You must choose correct part preference");

        RuleFor(x => x.TimelinePreference)
            .IsInEnum()
            .WithMessage("You must choose correct timeline preference");
    }
}
