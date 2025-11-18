
namespace eMechanic.Application.RepairRequest.Features.Accept;

using Common.CQRS;
using Common.Result;
using FluentValidation;
using MediatR;

public sealed record AcceptRepairEstimationCommand(Guid RepairRequestId) : IResultCommand<Success>;

public class AcceptRepairEstimationValidator : AbstractValidator<AcceptRepairEstimationCommand>
{
    public AcceptRepairEstimationValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
    }
}
