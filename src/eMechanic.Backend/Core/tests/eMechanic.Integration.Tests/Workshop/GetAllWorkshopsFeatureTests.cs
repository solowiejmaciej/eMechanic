using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using eMechanic.API.Constans;
using eMechanic.API.Features.Workshop;
using eMechanic.Integration.Tests.TestContainers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace eMechanic.Integration.Tests.Workshop;

using API.Features.Workshop.Create;
using Application.Workshop.Features.Get;
using Common.Result;

public class GetAllWorkshopsFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{WorkshopPrefix.GET_ALL_ENDPOINT}";
    private const string CREATE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}{WorkshopPrefix.CREATE_ENDPOINT}";

    public GetAllWorkshopsFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkshops_Should_ReturnOkAndList_WhenWorkshopsExist()
    {
        // Arrange
        await CreateTestWorkshop($"get-ws-1-{Guid.NewGuid()}@test.com");
        await CreateTestWorkshop($"get-ws-2-{Guid.NewGuid()}@test.com");

        // Act
        var response = await _client.GetAsync($"{BASE_API_URL}?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<PaginationResult<WorkshopResponse>>(content, options);
        list.Should().NotBeNull();
        list.Items.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetWorkshops_Should_ReturnOkAndEmptyList_WhenNoWorkshopsExist()
    {
        // Act
        var response = await _client.GetAsync($"{BASE_API_URL}?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNull();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<PaginationResult<WorkshopResponse>>(content, options);
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkshops_Should_ReturnBadRequest_WhenPaginationIsInvalid()
    {
        // Act
        var response = await _client.GetAsync($"{BASE_API_URL}?pageNumber=-1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().NotBeEmpty();
    }

    private async Task CreateTestWorkshop(string email)
    {
        var command = new CreateWorkshopRequest(
            email,
            "Password123!",
            $"contact-{Guid.NewGuid()}@workshop.com",
            "Test Workshop",
            $"TW-{Guid.NewGuid()}",
            "987654321",
            "ul. Testowa 1",
            "Miasto",
            "00-000",
            "Polska");

        var resp = await _client.PostAsJsonAsync(CREATE_API_URL, command);
        resp.EnsureSuccessStatusCode();
    }
}
