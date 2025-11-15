namespace eMechanic.Application.VehicleDocument.Features.Create;

using Common.CQRS;
using Domain.VehicleDocument.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

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
