namespace eMechanic.Application.VehicleDocument.Features.Delete;

using Common.CQRS;
using Common.Result;
using FluentValidation;
using System;

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
