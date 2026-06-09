namespace eMechanic.Application.RepairRequest.Features.Get.ById;

using Common.CQRS;
using Common.Result;

public sealed record GetRepairRequestByIdQuery(Guid Id) : IResultQuery<RepairRequestResponse>;
