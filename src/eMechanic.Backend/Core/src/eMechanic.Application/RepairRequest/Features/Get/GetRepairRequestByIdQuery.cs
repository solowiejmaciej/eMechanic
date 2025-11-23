
using eMechanic.Application.RepairRequest.Features.Get;
using eMechanic.Common.CQRS;

namespace eMechanic.Application.RepairRequest.Features.Get;

public sealed record GetRepairRequestByIdQuery(Guid Id) : IResultQuery<RepairRequestResponse>;
