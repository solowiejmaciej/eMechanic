namespace eMechanic.Application.Vehicle.Delete;

using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

public sealed record DeleteVehicleCommand(Guid Id) : IResultCommand<Success>;

public class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
{
    public DeleteVehicleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
    }
}
