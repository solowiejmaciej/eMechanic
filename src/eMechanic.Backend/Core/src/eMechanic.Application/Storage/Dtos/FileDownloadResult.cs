namespace eMechanic.Application.Storage.Dtos;

public record FileDownloadResult(
    Stream Content,
    string ContentType,
    string? FileName = null);
