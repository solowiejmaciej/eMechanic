namespace eMechanic.Infrastructure.Storage;

using Application.Abstractions.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Common.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Storage.Dtos;

internal sealed class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private static readonly char[] Separator = ['/'];
    private const int MAX_FILE_SIZE_IN_MB = 10 * 1024 * 1024;
    private static readonly string[] AllowedContentTypes =
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public AzureBlobStorageService(
        BlobServiceClient blobServiceClient,
        ILogger<AzureBlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    public async Task<Result<Success, Error>> UploadFileAsync(
        string fullPath,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return new Error(EErrorCode.ValidationError, "File is empty");
        }

        if (file.Length > MAX_FILE_SIZE_IN_MB)
        {
            return new Error(EErrorCode.ValidationError, "File is too large");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return new Error(EErrorCode.ValidationError, "Invalid ContentType");
        }

        try
        {
            var (containerName, blobName) = ResolvePath(fullPath);
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();
            var options = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType} };
            await blobClient.UploadAsync(stream, options, cancellationToken);

            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to upload file to {FullPath}", fullPath);
            return new Error(EErrorCode.InternalServerError, "Storage error");
        }
    }

    public async Task<Result<Success, Error>> DeleteFileAsync(
        string fullPath,
        CancellationToken cancellationToken)
    {
        try
        {
            var (containerName, blobName) = ResolvePath(fullPath);
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting file (Path: {FullPath})", fullPath);
            return new Error(EErrorCode.InternalServerError, "Storage error");
        }
    }

    public async Task<Result<FileDownloadResult, Error>> GetFileAsync(
        string fullPath,
        CancellationToken cancellationToken,
        string displayNameAs = "")
    {
        try
        {
            var (containerName, blobName) = ResolvePath(fullPath);
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                _logger.LogWarning("File doesn't exists {FullPath}", fullPath);
                return new Error(EErrorCode.NotFoundError, "File doesn't exists");
            }

            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

            var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);

            var fileName = blobName.Split('/').LastOrDefault() ?? blobName;

            if (!string.IsNullOrEmpty(displayNameAs))
            {
                fileName = displayNameAs;
            }

            return new FileDownloadResult(stream, properties.Value.ContentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while downloading file (Path: {FullPath})", fullPath);
            return new Error(EErrorCode.InternalServerError, "Storage error");
        }
    }

    private (string containerName, string blobName) ResolvePath(string fullPath, bool isDirectory = false)
    {
        var normalizedPath = fullPath.Replace("\\", "/").Trim('/');
        var parts = normalizedPath.Split(Separator, 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0 || string.IsNullOrEmpty(parts[0]))
        {
            throw new ArgumentException("Path must include container nam (ex. 'vehicle-documents').", nameof(fullPath));
        }

        var containerName = parts[0].ToLowerInvariant();
        var blobNameOrPrefix = parts.Length > 1 ? parts[1] : string.Empty;

        if (!isDirectory && string.IsNullOrEmpty(blobNameOrPrefix))
        {
            throw new ArgumentException("Full path must include container name and file name ex container/plik.jpg", nameof(fullPath));
        }

        return (containerName, blobNameOrPrefix);
    }
}
