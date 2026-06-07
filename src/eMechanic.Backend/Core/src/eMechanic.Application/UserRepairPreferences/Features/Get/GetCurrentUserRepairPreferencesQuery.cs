namespace eMechanic.Application.UserRepairPreferences.Features.Get;

using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetCurrentUserRepairPreferencesQuery() : IResultQuery<UserRepairPreferencesResponse>;
