namespace eMechanic.Application.Vehicle.Document.Features.Delete;

using System;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;

public sealed record DeleteVehicleDocumentCommand(
    Guid VehicleId,
    Guid DocumentId) : IResultCommand<Success>;

public class DeleteVehicleDocumentCommandValidator : AbstractValidator<DeleteVehicleDocumentCommand>
{
    public DeleteVehicleDocumentCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}
