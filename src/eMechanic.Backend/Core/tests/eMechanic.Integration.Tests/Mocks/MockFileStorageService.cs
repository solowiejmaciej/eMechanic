namespace eMechanic.Integration.Tests.Mocks;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Abstractions.Storage;
using eMechanic.Application.Storage.Dtos;
using eMechanic.Common.Result;
using Microsoft.AspNetCore.Http;

public class MockFileStorageService : IFileStorageService
{
    public readonly Dictionary<string, (byte[] Content, string ContentType)> Storage = new();

    public bool ShouldUploadFail { get; set; }
    public bool ShouldDeleteFail { get; set; }
    public bool ShouldGetFail { get; set; }

    public async Task<Result<Success, Error>> UploadFileAsync(
        string fullPath,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (ShouldUploadFail)
        {
            return new Error(EErrorCode.InternalServerError, "Mock storage upload error");
        }

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);

        Storage[fullPath] = (memoryStream.ToArray(), file.ContentType);

        return Result.Success;
    }

    public Task<Result<Success, Error>> DeleteFileAsync(
        string fullPath,
        CancellationToken cancellationToken)
    {
        if (ShouldDeleteFail)
        {
            return Task.FromResult<Result<Success, Error>>(
                new Error(EErrorCode.InternalServerError, "Mock storage delete error")
            );
        }

        Storage.Remove(fullPath);
        return Task.FromResult(Result.Success);
    }

    public Task<Result<FileDownloadResult, Error>> GetFileAsync(
        string fullPath,
        CancellationToken cancellationToken,
        string displayNameAs = "")
    {
        if (ShouldGetFail)
        {
            return Task.FromResult<Result<FileDownloadResult, Error>>(
                new Error(EErrorCode.InternalServerError, "Mock storage get error")
            );
        }

        if (!Storage.TryGetValue(fullPath, out var file))
        {
            return Task.FromResult<Result<FileDownloadResult, Error>>(
                new Error(EErrorCode.NotFoundError, "Mock file not found")
            );
        }

        var stream = new MemoryStream(file.Content);
        var fileName = string.IsNullOrEmpty(displayNameAs)
            ? fullPath.Split('/').LastOrDefault() ?? fullPath
            : displayNameAs;

        var downloadResult = new FileDownloadResult(stream, file.ContentType, fileName);

        return Task.FromResult<Result<FileDownloadResult, Error>>(downloadResult);
    }
}
