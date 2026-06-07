namespace eMechanic.Application.Payments.Features.Process;

using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;
using eMechanic.Application.Repair.Features.Get.ForUser;
using eMechanic.Application.Repair.Features.Get.ForWorkshop;
using eMechanic.Common.Cache.Attributes;

[InvalidatesCache(typeof(GetRepairsForUserQuery), typeof(GetRepairsForWorkshopQuery))]
public sealed record ProcessPaymentCommand(
    string JsonPayload,
    string SignatureHeader) : IResultCommand<Success>;

public sealed class ProcessPaymentCommandValidator
    : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.JsonPayload).NotEmpty();
        RuleFor(x => x.SignatureHeader).NotEmpty();
    }
}
