namespace eMechanic.Application.Vehicle.Document.Features.Create;

using Common.Cache.Attributes;
using Domain.Vehicle.Documents.Enums;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using FluentValidation;
using Get.All;
using Microsoft.AspNetCore.Http;

[InvalidatesCache(typeof(GetVehicleDocumentsQuery))]
public sealed record AddVehicleDocumentCommand(
    Guid VehicleId,
    IFormFile File,
    EVehicleDocumentType DocumentType) : IResultCommand<Guid>;

public class AddVehicleDocumentCommandValidator : AbstractValidator<AddVehicleDocumentCommand>
{
    private const long MAX_FILE_SIZE_IN_MB = 10 * 1024 * 1024;
    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public AddVehicleDocumentCommandValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();

        RuleFor(x => x.DocumentType)
            .IsInEnum()
            .NotEqual(EVehicleDocumentType.None)
            .WithMessage($"VehicleDocument type can't be {{{nameof(EVehicleDocumentType.None)}}}");

        RuleFor(x => x.File)
            .NotEmpty().WithMessage("File is required");

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(MAX_FILE_SIZE_IN_MB)
            .WithMessage($"File is to big (Max size is {MAX_FILE_SIZE_IN_MB / 1024 / 1024}MB).");

        RuleFor(x => x.File.ContentType)
            .Must(type => AllowedContentTypes.Contains(type))
            .WithMessage($"Invalid content type. Allowed types are: {string.Join(", ", AllowedContentTypes)}");
    }
}
