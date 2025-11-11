using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using eMechanic.Integration.Tests.TestContainers;

namespace eMechanic.Integration.Tests.Workshop;

using Application.Workshop.Features.Create;

public class CreateWorkshopFeatureTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    private CreateWorkshopCommand CreateValidCommand() => new(
        $"login-{Guid.NewGuid()}@workshop.com",
        "zaq1@WSX",
        $"contact-{Guid.NewGuid()}@workshop.com",
        "Super Warsztat",
        $"SuperW-{Guid.NewGuid()}",
        "987654321",
        "ul. Testowa 1",
        "Poznań",
        "60-123",
        "Polska");

    public CreateWorkshopFeatureTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkshop_Should_ReturnCreated_WhenDataIsValid()
    {
        var command = CreateValidCommand();
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var createdResponse = await response.Content.ReadFromJsonAsync<WorkshopIdResponse>();
        createdResponse.Should().NotBeNull();
        createdResponse!.WorkshopId.Should().NotBeEmpty();
    }

    private sealed record WorkshopIdResponse(Guid WorkshopId);

    [Fact]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenEmailIsNotUnique()
    {
        var uniqueEmail = $"duplicate-login-{Guid.NewGuid()}@workshop.com";
        var command1 = CreateValidCommand() with { Email = uniqueEmail };
        var response1 = await _client.PostAsJsonAsync("/api/v1/workshops", command1);
        response1.EnsureSuccessStatusCode();

        var command2 = CreateValidCommand() with { Email = uniqueEmail };
        var response2 = await _client.PostAsJsonAsync("/api/v1/workshops", command2);

        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response2.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenLoginEmailIsEmpty(string? invalidEmail)
    {
        var command = CreateValidCommand() with { Email = invalidEmail! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenPasswordIsEmpty(string? invalidPassword)
    {
        var command = CreateValidCommand() with { Password = invalidPassword! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Password");
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenContactEmailIsEmpty(string? invalidContactEmail)
    {
        var command = CreateValidCommand() with { ContactEmail = invalidContactEmail! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("ContactEmail");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenNameIsEmpty(string? invalidName)
    {
        var command = CreateValidCommand() with { Name = invalidName! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenDisplayNameIsEmpty(string? invalidDisplayName)
    {
        var command = CreateValidCommand() with { DisplayName = invalidDisplayName! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("DisplayName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenPhoneNumberIsEmpty(string? invalidPhone)
    {
        var command = CreateValidCommand() with { PhoneNumber = invalidPhone! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("PhoneNumber");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenAddressIsEmpty(string? invalidAddress)
    {
        var command = CreateValidCommand() with { Address = invalidAddress! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Address");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenCityIsEmpty(string? invalidCity)
    {
        var command = CreateValidCommand() with { City = invalidCity! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("City");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenPostalCodeIsEmpty(string? invalidPostalCode)
    {
        var command = CreateValidCommand() with { PostalCode = invalidPostalCode! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("PostalCode");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenCountryIsEmpty(string? invalidCountry)
    {
        var command = CreateValidCommand() with { Country = invalidCountry! };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Country");
    }

    [Theory]
    [InlineData("niepoprawny-email")]
    [InlineData("kontaktdasdadasdas.pl")]
    [InlineData("DSA")]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenEmailFormatIsInvalid(string invalidEmail)
    {
        var command = CreateValidCommand() with { Email = invalidEmail };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Email");
    }

    [Theory]
    [InlineData("niepoprawny-email")]
    [InlineData("kontaktdasdadasdas.pl")]
    [InlineData("DSA")]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenContactEmailFormatIsInvalid(string invalidContactEmail)
    {
        var command = CreateValidCommand() with { ContactEmail = invalidContactEmail };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("ContactEmail");
    }


    [Fact]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenPasswordIsTooShort()
    {
        var command = CreateValidCommand() with { Password = "zaq1" };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Password");
    }

    [Fact]
    public async Task CreateWorkshop_Should_ReturnBadRequest_WhenPasswordMissesDigit()
    {
        var command = CreateValidCommand() with { Password = "PasswordWithoutDigit" };
        var response = await _client.PostAsJsonAsync("/api/v1/workshops", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails!.Errors.Should().ContainKey("Password");
    }
}
