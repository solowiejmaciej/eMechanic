namespace eMechanic.Application.Vehicle.Vehicle.Features.Delete;

using Common.Cache.Attributes;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;
using Get.All;
using Get.ById;

[InvalidatesCache(typeof(GetVehiclesQuery), typeof(GetVehicleByIdQuery))]
public sealed record DeleteVehicleCommand(Guid Id) : IResultCommand<Success>;

public class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
{
    public DeleteVehicleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotEqual(Guid.Empty);
    }
}
