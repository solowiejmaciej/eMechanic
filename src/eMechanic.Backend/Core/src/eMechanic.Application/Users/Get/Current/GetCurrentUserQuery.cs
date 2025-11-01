namespace eMechanic.Application.Users.Get.Current;

using eMechanic.Common.CQRS;

public sealed record GetCurrentUserQuery() : IResultQuery<GetCurrentUserResponse>;
