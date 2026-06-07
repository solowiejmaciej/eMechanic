namespace eMechanic.Application.Workshop.Document.Features.Create;

using Common.Cache.Attributes;
using eMechanic.Common.Cache;
using eMechanic.Common.CQRS;
using eMechanic.Domain.Workshop.Documents.Enums;
using FluentValidation;
using Get;
using Microsoft.AspNetCore.Http;

[InvalidatesCache(typeof(GetWorkshopDocumentsQuery))]
public sealed record AddWorkshopDocumentCommand(
    IFormFile File,
    EWorkshopDocumentType DocumentType) : IResultCommand<Uri>;

public class AddWorkshopDocumentCommandValidator : AbstractValidator<AddWorkshopDocumentCommand>
{
    private const long MAX_FILE_SIZE_IN_MB = 10 * 1024 * 1024;
    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public AddWorkshopDocumentCommandValidator()
    {
        RuleFor(x => x.DocumentType)
            .IsInEnum()
            .NotEqual(EWorkshopDocumentType.None)
            .WithMessage($"VehicleDocument type can't be {{{nameof(EWorkshopDocumentType.None)}}}");

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
