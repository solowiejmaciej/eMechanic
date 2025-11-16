namespace eMechanic.Integration.Tests.VehicleDocument;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using API.Constans;
using API.Features.Vehicle;
using API.Features.Vehicle.Document;
using API.Features.Vehicle.Vehicle.Create.Request;
using Domain.Vehicle.Enums;
using Domain.VehicleDocument.Enums;
using eMechanic.Application.VehicleDocument.Features.Get;
using eMechanic.Common.Result;
using eMechanic.Integration.Tests.Helpers;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Mocks;

public class VehicleDocumentFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private readonly MockFileStorageService _storageMock;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    private readonly string _createVehicleUrl = $"{BASE_API_URL}{VehiclePrefix.CREATE}";

    private readonly Func<Guid, string> _docUploadListUrl = (vehicleId) =>
        $"{BASE_API_URL}{VehicleDocumentPrefix.ENDPOINT.Replace("{vehicleId:guid}", vehicleId.ToString())}";

    private readonly Func<Guid, Guid, string> _docDownloadUrl = (vId, dId) =>
        $"{BASE_API_URL}{VehicleDocumentPrefix.DOWNLOAD
            .Replace("{vehicleId:guid}", vId.ToString())
            .Replace("{documentId:guid}", dId.ToString())}";

    private readonly Func<Guid, Guid, string> _docDeleteUrl = (vId, dId) =>
        $"{BASE_API_URL}{VehicleDocumentPrefix.DELETE
            .Replace("{vehicleId:guid}", vId.ToString())
            .Replace("{documentId:guid}", dId.ToString())}";

    public VehicleDocumentFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);

        _storageMock = factory.Services.GetRequiredService<Application.Abstractions.Storage.IFileStorageService>() as MockFileStorageService
                       ?? throw new InvalidOperationException("IFileStorageService is not MockFileStorageService. Check IntegrationTestWebAppFactory.");

        _storageMock.Storage.Clear();
        _storageMock.ShouldUploadFail = false;
        _storageMock.ShouldDeleteFail = false;
        _storageMock.ShouldGetFail = false;
    }

    private async Task<(Guid VehicleId, string Token)> CreateVehicleForTest()
    {
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"user-doc-test-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        var createRequest = new CreateVehicleRequest(
            $"V1N{Guid.NewGuid().ToString("N")[..14]}", "DocTest", "DocModel", "2022", 1.6m, 15000,
            EMileageUnit.Kilometers, "DOC1234", 120, EFuelType.Gasoline, EBodyType.Hatchback, EVehicleType.Passenger);

        // Użycie poprawionego URL
        var createResponse = await _client.PostAsJsonAsync(_createVehicleUrl, createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdContent = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();

        return (createdContent!["vehicleId"], authResponse.Token);
    }

    private MultipartFormDataContent CreateTestFileContent(EVehicleDocumentType docType, string contentType = "image/jpeg", string fileName = "test-image.jpg", byte[]? contentBytes = null)
    {
        var content = new MultipartFormDataContent();
        var fileBytes = contentBytes ?? [1, 2, 3, 4, 5];
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(((int)docType).ToString(System.Globalization.CultureInfo.InvariantCulture)), "documentType");
        return content;
    }

    [Fact]
    public async Task DocumentLifecycle_Should_Succeed_When_UserIsOwnerAndDataIsValid()
    {
        // ARRANGE
        var (vehicleId, token) = await CreateVehicleForTest();
        _client.SetBearerToken(token);
        var url = _docUploadListUrl(vehicleId);

        // ACT 1: UPLOAD
        var fileContent = CreateTestFileContent(EVehicleDocumentType.Photo, "image/jpeg", "test-image.jpg");
        var uploadResponse = await _client.PostAsync(url, fileContent);

        // ASSERT 1: UPLOAD
        uploadResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var location = uploadResponse.Headers.Location;
        location.Should().NotBeNull();
        _storageMock.Storage.Count.Should().Be(1);
        _storageMock.Storage.First().Value.ContentType.Should().Be("image/jpeg");

        var locationString = location!.ToString();
        var parts = locationString.Split('/');
        var documentId = parts.ElementAt(parts.Length - 2);
        Guid.TryParse(documentId, out var documentGuid).Should().BeTrue("GUID z 'Location' header powinien być poprawny");

        // ACT 2: GET LIST
        var getListResponse = await _client.GetAsync($"{url}?pageNumber=1&pageSize=10");

        // ASSERT 2: GET LIST
        getListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await getListResponse.Content.ReadFromJsonAsync<PaginationResult<VehicleDocumentResponse>>();
        list!.Items.Count().Should().Be(1);
        list.Items.First().OriginalFileName.Should().Be("test-image.jpg");
        list.Items.First().DocumentId.ToString().Should().Be(documentId);
        list.Items.First().DocumentType.Should().Be(EVehicleDocumentType.Photo);

        // ACT 3: DOWNLOAD
        var downloadUrl = _docDownloadUrl(vehicleId, documentGuid);
        var downloadResponse = await _client.GetAsync(downloadUrl);

        // ASSERT 3: DOWNLOAD
        downloadResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        downloadResponse.Content.Headers.ContentType!.ToString().Should().Be("image/jpeg");
        downloadResponse.Content.Headers.ContentDisposition!.FileName.Should().Be("test-image.jpg");
        var downloadedBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
        downloadedBytes.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });

        // ACT 4: DELETE
        var deleteUrl = _docDeleteUrl(vehicleId, documentGuid);
        var deleteResponse = await _client.DeleteAsync(deleteUrl);

        // ASSERT 4: DELETE
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _storageMock.Storage.Count.Should().Be(0);

        // ACT 5: VERIFY DELETE
        var getListAfterDeleteResponse = await _client.GetAsync($"{url}?pageNumber=1&pageSize=10");
        var listAfterDelete = await getListAfterDeleteResponse.Content.ReadFromJsonAsync<PaginationResult<VehicleDocumentResponse>>();
        listAfterDelete!.Items.Count().Should().Be(0);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Upload_Should_ReturnUnauthorized_When_TokenIsMissing()
    {
        // Arrange
        var url = _docUploadListUrl(Guid.NewGuid());
        var fileContent = CreateTestFileContent(EVehicleDocumentType.Photo);
        _client.ClearBearerToken();

        // Act
        var response = await _client.PostAsync(url, fileContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Upload_Should_ReturnForbidden_When_WorkshopTokenIsUsed()
    {
        // Arrange
        var (vehicleId, _) = await CreateVehicleForTest();
        _client.ClearBearerToken();

        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(workshopAuth.Token);

        var fileContent = CreateTestFileContent(EVehicleDocumentType.Photo);

        // Act
        var response = await _client.PostAsync(_docUploadListUrl(vehicleId), fileContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_DocumentBelongsToAnotherUser()
    {
        // ARRANGE
        // UserA tworzy pojazd i wgrywa plik
        var (vehicleIdUserA, tokenA) = await CreateVehicleForTest();
        _client.SetBearerToken(tokenA);
        var fileContent = CreateTestFileContent(EVehicleDocumentType.Photo);
        var uploadResponse = await _client.PostAsync(_docUploadListUrl(vehicleIdUserA), fileContent);
        var location = uploadResponse.Headers.Location;

        var parts = location!.ToString().Split('/');
        var documentId = parts.ElementAt(parts.Length - 2);
        Guid.TryParse(documentId, out var documentGuid).Should().BeTrue();

        _client.ClearBearerToken();

        var authResponseB = await _authHelper.CreateAndLoginUserAsync($"userB-doc-test-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponseB.Token);

        // ACT
        var deleteUrl = _docDeleteUrl(vehicleIdUserA, documentGuid);
        var deleteResponse = await _client.DeleteAsync(deleteUrl);

        // ASSERT
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _storageMock.Storage.Count.Should().Be(1);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Download_Should_ReturnNotFound_When_DocumentBelongsToAnotherUser()
    {
        // ARRANGE
        var (vehicleIdUserA, tokenA) = await CreateVehicleForTest();
        _client.SetBearerToken(tokenA);
        var fileContent = CreateTestFileContent(EVehicleDocumentType.Photo);
        var uploadResponse = await _client.PostAsync(_docUploadListUrl(vehicleIdUserA), fileContent);
        var location = uploadResponse.Headers.Location;

        var parts = location!.ToString().Split('/');
        var documentId = parts.ElementAt(parts.Length - 2);
        Guid.TryParse(documentId, out var documentGuid).Should().BeTrue();

        _client.ClearBearerToken();

        var authResponseB = await _authHelper.CreateAndLoginUserAsync($"userB-doc-test-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponseB.Token);

        // ACT
        var downloadUrl = _docDownloadUrl(vehicleIdUserA, documentGuid);
        var downloadResponse = await _client.GetAsync(downloadUrl);

        // ASSERT
        downloadResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("text/plain", "invalid.txt")]
    [InlineData("application/json", "data.json")]
    public async Task Upload_Should_ReturnBadRequest_When_FileContentTypeIsInvalid(string contentType, string fileName)
    {
        // Arrange
        var (vehicleId, token) = await CreateVehicleForTest();
        _client.SetBearerToken(token);
        var fileContent = CreateTestFileContent(EVehicleDocumentType.Other, contentType, fileName);

        // Act
        var response = await _client.PostAsync(_docUploadListUrl(vehicleId), fileContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("File.ContentType");
        _storageMock.Storage.Count.Should().Be(0);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Upload_Should_ReturnBadRequest_When_DocumentTypeIsNone()
    {
        // Arrange
        var (vehicleId, token) = await CreateVehicleForTest();
        _client.SetBearerToken(token);
        var fileContent = CreateTestFileContent(EVehicleDocumentType.None);

        // Act
        var response = await _client.PostAsync(_docUploadListUrl(vehicleId), fileContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("DocumentType");

        _client.ClearBearerToken();
    }
}
