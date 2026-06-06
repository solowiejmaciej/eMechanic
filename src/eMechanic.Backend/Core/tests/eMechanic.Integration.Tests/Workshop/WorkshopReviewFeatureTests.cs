namespace eMechanic.Integration.Tests.Workshop;

using System.Net;
using System.Net.Http.Json;
using API.Features.Workshop;
using API.Features.Workshop.Reviews.Request;
using Application.Workshop.Reviews.Features.Get;
using Common.Result;
using FluentAssertions;
using Helpers;
using TestContainers;

[Collection("Sequential")]
public class WorkshopReviewFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;

    private const string BASE_API_URL = "/api/v1";

    public WorkshopReviewFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task UpsertWorkshopReview_Should_CreateAndUpdateReview_ForCurrentUser()
    {
        // Arrange
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(userAuth.Token);

        var upsertUrl = $"{BASE_API_URL}{WorkshopPrefix.UPSERT_WORKSHOP_REVIEW_ENDPOINT.Replace("{workshopId}", workshopAuth.DomainId.ToString())}";

        // Act
        var createResponse = await _client.PutAsJsonAsync(upsertUrl, new UpsertWorkshopReviewRequest(5, "Excellent"));
        var updateResponse = await _client.PutAsJsonAsync(upsertUrl, new UpsertWorkshopReviewRequest(4, "Still good"));

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var secondId = await updateResponse.Content.ReadFromJsonAsync<Guid>();
        secondId.Should().Be(firstId);
    }

    [Fact]
    public async Task GetWorkshopReviews_Should_ReturnPaginatedReviews()
    {
        // Arrange
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();
        var firstUser = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(firstUser.Token);

        var workshopId = workshopAuth.DomainId;
        var upsertUrl = $"{BASE_API_URL}{WorkshopPrefix.UPSERT_WORKSHOP_REVIEW_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";
        await _client.PutAsJsonAsync(upsertUrl, new UpsertWorkshopReviewRequest(5, "Great team"));

        var secondUser = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(secondUser.Token);
        await _client.PutAsJsonAsync(upsertUrl, new UpsertWorkshopReviewRequest(2, "Slow service"));

        _client.ClearBearerToken();
        var listUrl = $"{BASE_API_URL}{WorkshopPrefix.GET_WORKSHOP_REVIEWS_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}?pageNumber=1&pageSize=10&searchPhrase=great";

        // Act
        var listResponse = await _client.GetAsync(listUrl);

        // Assert
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await listResponse.Content.ReadFromJsonAsync<PaginationResult<WorkshopReviewResponse>>();
        payload.Should().NotBeNull();
        payload.Items.Should().ContainSingle();
        payload.Items.First().Rating.Should().Be(5);
        payload.Items.First().Comment.Should().Be("Great team");
    }

    [Fact]
    public async Task GetWorkshopReviewStats_Should_ReturnAverageAndCount()
    {
        // Arrange
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();
        var firstUser = await _authHelper.CreateAndLoginUserAsync();
        var workshopId = workshopAuth.DomainId;

        var upsertUrl = $"{BASE_API_URL}{WorkshopPrefix.UPSERT_WORKSHOP_REVIEW_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";

        _client.SetBearerToken(firstUser.Token);
        await _client.PutAsJsonAsync(upsertUrl, new UpsertWorkshopReviewRequest(5, null));

        var secondUser = await _authHelper.CreateAndLoginUserAsync();
        _client.SetBearerToken(secondUser.Token);
        await _client.PutAsJsonAsync(upsertUrl, new UpsertWorkshopReviewRequest(3, null));

        _client.ClearBearerToken();
        var statsUrl = $"{BASE_API_URL}{WorkshopPrefix.GET_WORKSHOP_REVIEW_STATS_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";

        // Act
        var response = await _client.GetAsync(statsUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<WorkshopReviewStatsResponse>();
        stats.Should().NotBeNull();
        stats.WorkshopId.Should().Be(workshopId);
        stats.TotalReviews.Should().Be(2);
        stats.AverageRating.Should().Be(4m);
    }

    [Fact]
    public async Task DeleteWorkshopReview_Should_ReturnNoContent_WhenReviewExists()
    {
        // Arrange
        var workshopAuth = await _authHelper.CreateAndLoginWorkshopAsync();
        var userAuth = await _authHelper.CreateAndLoginUserAsync();
        var workshopId = workshopAuth.DomainId;

        _client.SetBearerToken(userAuth.Token);
        var reviewUrl = $"{BASE_API_URL}{WorkshopPrefix.UPSERT_WORKSHOP_REVIEW_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";
        await _client.PutAsJsonAsync(reviewUrl, new UpsertWorkshopReviewRequest(2, "Needs improvement"));

        var deleteUrl = $"{BASE_API_URL}{WorkshopPrefix.WORKSHOP_REVIEWS_ENDPOINT.Replace("{workshopId}", workshopId.ToString())}";

        // Act
        var deleteResponse = await _client.DeleteAsync(deleteUrl);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
