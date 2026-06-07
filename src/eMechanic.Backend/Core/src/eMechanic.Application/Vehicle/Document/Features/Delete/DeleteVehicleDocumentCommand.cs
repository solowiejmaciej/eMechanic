namespace eMechanic.Application.Vehicle.Document.Features.Delete;

using System;
using Common.Cache.Attributes;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using FluentValidation;
using Get.All;

[InvalidatesCache(typeof(GetVehicleDocumentsQuery))]
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
