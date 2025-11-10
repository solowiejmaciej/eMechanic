namespace eMechanic.Application.Users.Features.Get.Current;

using eMechanic.Common.CQRS;

public sealed record GetCurrentUserQuery() : IResultQuery<GetCurrentUserResponse>;
