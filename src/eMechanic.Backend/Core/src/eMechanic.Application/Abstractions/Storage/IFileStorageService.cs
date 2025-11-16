namespace eMechanic.Application.Abstractions.Storage;

using eMechanic.Common.Result;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Application.Storage.Dtos;

public interface IFileStorageService
{
    Task<Result<Success, Error>> UploadFileAsync(
        string fullPath,
        IFormFile file,
        CancellationToken cancellationToken);

    Task<Result<Success, Error>> DeleteFileAsync(
        string fullPath,
        CancellationToken cancellationToken);

    Task<Result<FileDownloadResult, Error>> GetFileAsync(
        string fullPath,
        CancellationToken cancellationToken,
        string displayNameAs = "");
}
