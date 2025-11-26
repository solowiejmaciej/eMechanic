namespace eMechanic.Integration.Tests.WorkshopDocument;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using eMechanic.API.Constans;
using eMechanic.API.Features.Workshop;
using eMechanic.API.Features.Workshop.Document;
using eMechanic.API.Features.Workshop.Workshop.Create;
using eMechanic.Application.Workshop.Document.Features.Get;
using eMechanic.Common.Result;
using eMechanic.Domain.Workshop.Documents.Enums;
using eMechanic.Integration.Tests.Helpers;
using eMechanic.Integration.Tests.Mocks;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

public class WorkshopDocumentFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private readonly MockFileStorageService _storageMock;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";

    private readonly string _docUploadUrl = $"{BASE_API_URL}{WorkshopDocumentPrefix.ENDPOINT}";

    private readonly Func<Guid, string> _docGetListUrl = (workshopId) =>
        $"{BASE_API_URL}{WorkshopDocumentPrefix.GET_ALL.Replace("{workshopId:guid}", workshopId.ToString())}?pageNumber=1&pageSize=100";

    private readonly Func<Guid, string> _docDeleteUrl = (docId) =>
        $"{BASE_API_URL}{WorkshopDocumentPrefix.DELETE.Replace("{documentId:guid}", docId.ToString())}";

    public WorkshopDocumentFeatureTests(IntegrationTestWebAppFactory factory)
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

    private async Task<(Guid WorkshopId, string Token)> CreateWorkshopForTest()
    {
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync($"workshop-doc-test-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);
        return (authResponse.DomainId, authResponse.Token);
    }

    private MultipartFormDataContent CreateTestFileContent(EWorkshopDocumentType docType, string contentType = "image/jpeg", string fileName = "workshop-logo.jpg", byte[]? contentBytes = null)
    {
        var content = new MultipartFormDataContent();
        var fileBytes = contentBytes ?? [10, 20, 30, 40, 50];
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(((int)docType).ToString(System.Globalization.CultureInfo.InvariantCulture)), "documentType");
        return content;
    }

    [Fact]
    public async Task DocumentLifecycle_Should_Succeed_When_WorkshopIsOwnerAndDataIsValid()
    {
        var (workshopId, token) = await CreateWorkshopForTest();
        _client.SetBearerToken(token);

        var fileContent = CreateTestFileContent(EWorkshopDocumentType.Logo, "image/png", "logo.png");
        var uploadResponse = await _client.PostAsync(_docUploadUrl, fileContent);

        uploadResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var location = uploadResponse.Headers.Location;
        location.Should().NotBeNull();
        _storageMock.Storage.Count.Should().Be(1);
        _storageMock.Storage.First().Value.ContentType.Should().Be("image/png");

        var uploadBody = await uploadResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        uploadBody.Should().ContainKey("publicUrl");

        _client.ClearBearerToken();
        var listUrl = _docGetListUrl(workshopId);
        var getListResponse = await _client.GetAsync(listUrl);

        getListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await getListResponse.Content.ReadFromJsonAsync<PaginationResult<WorkshopDocumentResponse>>();
        list!.Items.Count().Should().Be(1);
        var uploadedDoc = list.Items.First();

        uploadedDoc.FileName.Should().Be("logo.png");
        uploadedDoc.Type.Should().Be(EWorkshopDocumentType.Logo);
        uploadedDoc.PublicUrl.ToString().Should().NotBeNullOrEmpty();

        var documentId = uploadedDoc.Id;

        _client.SetBearerToken(token);
        var deleteUrl = _docDeleteUrl(documentId);
        var deleteResponse = await _client.DeleteAsync(deleteUrl);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        _storageMock.Storage.Count.Should().Be(0);

        var getListAfterDeleteResponse = await _client.GetAsync(listUrl);
        var listAfterDelete = await getListAfterDeleteResponse.Content.ReadFromJsonAsync<PaginationResult<WorkshopDocumentResponse>>();
        listAfterDelete!.Items.Count().Should().Be(0);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Upload_Should_ReturnUnauthorized_When_TokenIsMissing()
    {
        var fileContent = CreateTestFileContent(EWorkshopDocumentType.Certificate);
        _client.ClearBearerToken();

        var response = await _client.PostAsync(_docUploadUrl, fileContent);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Upload_Should_ReturnForbidden_When_UserTokenIsUsedInsteadOfWorkshop()
    {
        var userAuth = await _authHelper.CreateAndLoginUserAsync($"user-trying-workshop-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(userAuth.Token);

        var fileContent = CreateTestFileContent(EWorkshopDocumentType.Certificate);

        var response = await _client.PostAsync(_docUploadUrl, fileContent);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_DocumentBelongsToAnotherWorkshop()
    {
        var (workshopIdA, tokenA) = await CreateWorkshopForTest();
        _client.SetBearerToken(tokenA);

        var fileContent = CreateTestFileContent(EWorkshopDocumentType.Other);
        await _client.PostAsync(_docUploadUrl, fileContent);

        var listResponse = await _client.GetAsync(_docGetListUrl(workshopIdA));
        var list = await listResponse.Content.ReadFromJsonAsync<PaginationResult<WorkshopDocumentResponse>>();
        var docId = list!.Items.First().Id;

        _client.ClearBearerToken();

        var (workshopIdB, tokenB) = await CreateWorkshopForTest();
        _client.SetBearerToken(tokenB);

        var deleteResponse = await _client.DeleteAsync(_docDeleteUrl(docId));

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        _storageMock.Storage.Count.Should().Be(1);

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData("text/plain", "invalid.txt")]
    [InlineData("application/json", "data.json")]
    public async Task Upload_Should_ReturnBadRequest_When_FileContentTypeIsInvalid(string contentType, string fileName)
    {
        var (_, token) = await CreateWorkshopForTest();
        _client.SetBearerToken(token);
        var fileContent = CreateTestFileContent(EWorkshopDocumentType.Other, contentType, fileName);

        var response = await _client.PostAsync(_docUploadUrl, fileContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("File.ContentType");
        _storageMock.Storage.Count.Should().Be(0);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Upload_Should_ReturnBadRequest_When_DocumentTypeIsNone()
    {
        var (_, token) = await CreateWorkshopForTest();
        _client.SetBearerToken(token);
        var fileContent = CreateTestFileContent(EWorkshopDocumentType.None);

        var response = await _client.PostAsync(_docUploadUrl, fileContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("DocumentType");

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task Get_Should_ReturnOk_And_BePubliclyAccessible()
    {
        var (workshopId, token) = await CreateWorkshopForTest();
        _client.SetBearerToken(token);

        await _client.PostAsync(_docUploadUrl, CreateTestFileContent(EWorkshopDocumentType.Logo));
        _client.ClearBearerToken();

        var response = await _client.GetAsync($"{_docGetListUrl(workshopId)}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginationResult<WorkshopDocumentResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
    }
}
