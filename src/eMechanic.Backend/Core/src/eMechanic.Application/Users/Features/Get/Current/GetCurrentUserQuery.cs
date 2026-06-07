namespace eMechanic.Application.Users.Features.Get.Current;

using Common.Cache;
using Common.Cache.Attributes;
using Common.Cache.Configuration;
using eMechanic.Common.CQRS;

[Cache(CacheDefaults.DEFAULT_DURATION_SECONDS, ECacheScope.User)]
public sealed record GetCurrentUserQuery() : IResultQuery<GetCurrentUserResponse>;
