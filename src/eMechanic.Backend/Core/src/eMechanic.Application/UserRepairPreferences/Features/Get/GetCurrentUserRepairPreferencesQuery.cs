namespace eMechanic.Application.UserRepairPreferences.Features.Get;

using eMechanic.Common.CQRS;

public sealed record GetCurrentUserRepairPreferencesQuery() : IResultQuery<UserRepairPreferencesResponse>;
