namespace eMechanic.Infrastructure.Tests.Services.Identity.ExternalProviders.Google;

using System.Security.Claims;
using eMechanic.Application.Abstractions.Identity;
using eMechanic.Application.Identity;
using eMechanic.Application.Users.Services;
using eMechanic.Application.Users.Repositories;
using eMechanic.Common.Result;
using eMechanic.Infrastructure.Services.Identity.ExternalProviders.Google;
using FluentAssertions;
using global::Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

public class GoogleAuthServiceTests
{
    private readonly UserManager<Infrastructure.Identity.Identity> _userManagerMock;
    private readonly IUserService _userServiceMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly ILogger<GoogleAuthService> _loggerMock;
    private readonly IGoogleTokenValidator _googleTokenValidatorMock;
    private readonly GoogleAuthService _sut;

    public GoogleAuthServiceTests()
    {
        var userStoreMock = Substitute.For<IUserStore<Infrastructure.Identity.Identity>>();
        _userManagerMock = Substitute.For<UserManager<Infrastructure.Identity.Identity>>(
            userStoreMock, null, null, null, null, null, null, null, null);

        _userServiceMock = Substitute.For<IUserService>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _loggerMock = Substitute.For<ILogger<GoogleAuthService>>();
        _googleTokenValidatorMock = Substitute.For<IGoogleTokenValidator>();

        _sut = new GoogleAuthService(
            _userManagerMock,
            _userServiceMock,
            _userRepositoryMock,
            _loggerMock,
            _googleTokenValidatorMock
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnValidationError_WhenPayloadIsNull()
    {
        _googleTokenValidatorMock.ValidateAndGetPayload(Arg.Any<string>())
            .Returns((GoogleJsonWebSignature.Payload?)null);

        var result = await _sut.LoginAsync("invalid_token", CancellationToken.None);

        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNotImplemented_WhenUserIsWorkshop()
    {
        var payload = new GoogleJsonWebSignature.Payload { Email = "workshop@test.com", Subject = "123" };
        var existingIdentity = Infrastructure.Identity.Identity.Create(payload.Email, EIdentityType.Workshop);

        _googleTokenValidatorMock.ValidateAndGetPayload(Arg.Any<string>()).Returns(payload);
        _userManagerMock.FindByEmailAsync(payload.Email).Returns(existingIdentity);

        var result = await _sut.LoginAsync("token", CancellationToken.None);

        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotImplementedError);
    }

    [Fact]
    public async Task LoginAsync_ShouldCreateNewUser_WhenUserDoesNotExist()
    {
        var payload = new GoogleJsonWebSignature.Payload
        {
            Email = "newuser@test.com",
            Subject = "google_id_123",
            GivenName = "John",
            FamilyName = "Doe"
        };

        var newUserId = Guid.NewGuid();
        var newIdentityId = Guid.NewGuid();

        _googleTokenValidatorMock.ValidateAndGetPayload(Arg.Any<string>()).Returns(payload);
        _userManagerMock.FindByEmailAsync(payload.Email).Returns((Infrastructure.Identity.Identity?)null);

        Result<(Guid, Guid), Error> successResult = (newUserId, newIdentityId);

        _userServiceMock.CreateUserWithIdentityAsync(
            payload.Email,
            Arg.Any<string>(),
            payload.GivenName,
            payload.FamilyName,
            "Google",
            payload.Subject,
            Arg.Any<CancellationToken>()
        ).Returns(successResult);

        var result = await _sut.LoginAsync("token", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DomainEntityId.Should().Be(newUserId);
        result.Value.IdentityId.Should().Be(newIdentityId);
        result.Value.Email.Should().Be(payload.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldLinkGoogleLogin_WhenUserExistsButNotLinked()
    {
        var payload = new GoogleJsonWebSignature.Payload { Email = "existing@test.com", Subject = "google_id_123", GivenName = "John", FamilyName = "Doe" };
        var identityId = Guid.NewGuid();

        var existingIdentity = Infrastructure.Identity.Identity.Create(payload.Email, EIdentityType.User);
        existingIdentity.Id = identityId;

        var domainUser = eMechanic.Domain.User.User.Create(payload.Email, "John", "Doe", identityId);

        _googleTokenValidatorMock.ValidateAndGetPayload(Arg.Any<string>()).Returns(payload);

        _userManagerMock.FindByEmailAsync(payload.Email).Returns(existingIdentity);

        _userManagerMock.GetLoginsAsync(existingIdentity).Returns(new List<UserLoginInfo>());

        _userManagerMock.AddLoginAsync(existingIdentity, Arg.Is<UserLoginInfo>(x => x.LoginProvider == "Google"))
            .Returns(IdentityResult.Success);

        _userRepositoryMock.GetByIdentityIdAsync(identityId).Returns(domainUser);

        var result = await _sut.LoginAsync("token", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DomainEntityId.Should().Be(domainUser.Id);
        await _userManagerMock.Received(1).AddLoginAsync(existingIdentity, Arg.Is<UserLoginInfo>(l => l.ProviderKey == payload.Subject));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnValidationError_WhenLinkingFails()
    {
        var payload = new GoogleJsonWebSignature.Payload { Email = "existing@test.com", Subject = "123" };
        var existingIdentity = Infrastructure.Identity.Identity.Create(payload.Email, EIdentityType.User);
        existingIdentity.Id = Guid.NewGuid();

        _googleTokenValidatorMock.ValidateAndGetPayload(Arg.Any<string>()).Returns(payload);
        _userManagerMock.FindByEmailAsync(payload.Email).Returns(existingIdentity);
        _userManagerMock.GetLoginsAsync(existingIdentity).Returns(new List<UserLoginInfo>());

        _userManagerMock.AddLoginAsync(existingIdentity, Arg.Any<UserLoginInfo>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "DB Error" }));

        var result = await _sut.LoginAsync("token", CancellationToken.None);

        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
    }

    [Fact]
    public async Task LoginAsync_ShouldCatchException_AndLogIt()
    {
        var exception = new InvalidOperationException("Critical Google Failure");

        _googleTokenValidatorMock.ValidateAndGetPayload(Arg.Any<string>()).Throws(exception);

        var result = await _sut.LoginAsync("token", CancellationToken.None);

        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);

        _loggerMock.ReceivedWithAnyArgs().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
