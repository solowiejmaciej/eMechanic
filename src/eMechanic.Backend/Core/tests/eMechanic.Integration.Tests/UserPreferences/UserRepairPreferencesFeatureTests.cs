namespace eMechanic.Integration.Tests.UserPreferences;

using System.Net;
using System.Net.Http.Json;
using API.Features.UserRepairPreferences.Update.Request;
using Application.UserRepairPreferences.Features.Get;
using Domain.UserRepairPreferences.Enums;
using eMechanic.API.Constans;
using eMechanic.API.Features.UserRepairPreferences;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TestContainers;

public class UserRepairPreferencesFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly AuthHelper _authHelper;
    private const string BASE_API_URL = $"/api/{WebApiConstans.CURRENT_API_VERSION}";
    private readonly string _userRepairPreferencesUrl = $"{BASE_API_URL}{UserPreferencesPrefix.GET_ENDPOINT}";

    public UserRepairPreferencesFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authHelper = new AuthHelper(_client);
    }

    [Fact]
    public async Task GetAndUpdate_Preferences_Should_Succeed_When_FlowIsCorrect()
    {
        // ARRANGE
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"prefs-user-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        // ACT (GET)
        var getResponse = await _client.GetAsync(_userRepairPreferencesUrl);

        // ASSERT (GET)
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var preferences = await getResponse.Content.ReadFromJsonAsync<UserRepairPreferencesResponse>();

        preferences.Should().NotBeNull();
        preferences!.UserId.Should().Be(authResponse.DomainId);
        preferences.PartsPreference.Should().Be(EPartsPreference.Balanced);
        preferences.TimelinePreference.Should().Be(ETimelinePreference.Standard);

        // ARRANGE (PUT)
        var updateRequest = new UpdateUserRepairPreferencesRequest(
            EPartsPreference.Premium,
            ETimelinePreference.Urgent
        );

        var putResponse = await _client.PutAsJsonAsync(_userRepairPreferencesUrl, updateRequest);

        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // ACT (Verify PUT)
        var verifyResponse = await _client.GetAsync(_userRepairPreferencesUrl);

        // ASSERT (Verify PUT)
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedPreferences = await verifyResponse.Content.ReadFromJsonAsync<UserRepairPreferencesResponse>();

        updatedPreferences.Should().NotBeNull();
        updatedPreferences!.PartsPreference.Should().Be(EPartsPreference.Premium);
        updatedPreferences.TimelinePreference.Should().Be(ETimelinePreference.Urgent);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task GetPreferences_Should_ReturnUnauthorized_When_TokenIsMissing()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(_userRepairPreferencesUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePreferences_Should_ReturnUnauthorized_When_TokenIsMissing()
    {
        // Arrange
        _client.ClearBearerToken();
        var updateRequest = new UpdateUserRepairPreferencesRequest(EPartsPreference.Premium, ETimelinePreference.Urgent);

        // Act
        var response = await _client.PutAsJsonAsync(_userRepairPreferencesUrl, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPreferences_Should_ReturnForbidden_When_WorkshopTokenIsUsed()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);

        // Act
        var response = await _client.GetAsync(_userRepairPreferencesUrl);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }

    [Fact]
    public async Task UpdatePreferences_Should_ReturnForbidden_When_WorkshopTokenIsUsed()
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginWorkshopAsync();
        _client.SetBearerToken(authResponse.Token);
        var updateRequest = new UpdateUserRepairPreferencesRequest(EPartsPreference.Premium, ETimelinePreference.Urgent);

        // Act
        var response = await _client.PutAsJsonAsync(_userRepairPreferencesUrl, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        _client.ClearBearerToken();
    }

    [Theory]
    [InlineData((EPartsPreference)99, ETimelinePreference.Standard)]
    [InlineData(EPartsPreference.Balanced, (ETimelinePreference)99)]
    [InlineData((EPartsPreference)99, (ETimelinePreference)99)]
    public async Task UpdatePreferences_Should_ReturnBadRequest_When_ValidationFails(
        EPartsPreference partsPref, ETimelinePreference timelinePref)
    {
        // Arrange
        var authResponse = await _authHelper.CreateAndLoginUserAsync($"prefs-validation-{Guid.NewGuid()}@int.com");
        _client.SetBearerToken(authResponse.Token);

        var updateRequest = new UpdateUserRepairPreferencesRequest(partsPref, timelinePref);

        // Act
        var response = await _client.PutAsJsonAsync(_userRepairPreferencesUrl, updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();

        _client.ClearBearerToken();
    }
}
