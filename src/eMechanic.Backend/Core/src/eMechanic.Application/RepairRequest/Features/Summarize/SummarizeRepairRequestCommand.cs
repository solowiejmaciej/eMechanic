namespace eMechanic.Application.RepairRequest.Features.Summarize;

using Common.CQRS;
using FluentValidation;

public sealed record SummarizeRepairRequestCommand(Guid RepairRequestId) : IResultCommand<string>;

public class SummarizeRepairRequestValidator : AbstractValidator<SummarizeRepairRequestCommand>
{
    public SummarizeRepairRequestValidator()
    {
        RuleFor(x => x.RepairRequestId).NotEmpty();
    }
}
