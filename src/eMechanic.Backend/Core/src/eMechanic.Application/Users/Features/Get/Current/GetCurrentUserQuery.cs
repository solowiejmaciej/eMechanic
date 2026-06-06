namespace eMechanic.Application.Users.Features.Get.Current;

using Common.Cache;
using eMechanic.Common.CQRS;

[Cache(300, ECacheScope.User)]
public sealed record GetCurrentUserQuery() : IResultQuery<GetCurrentUserResponse>;
