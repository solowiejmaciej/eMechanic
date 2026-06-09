namespace eMechanic.Application.UserRepairPreferences.Features.Get;

using eMechanic.Common.CQRS;
using FluentValidation;

public sealed record GetUserRepairPreferencesQuery(Guid UserId)
    : IResultQuery<UserRepairPreferencesResponse>;

public sealed class GetUserRepairPreferencesQueryValidator : AbstractValidator<GetUserRepairPreferencesQuery>
{
    public GetUserRepairPreferencesQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
