namespace eMechanic.Infrastructure.Tests.Services.Identity;

using Application.Abstractions.User;
using Application.Abstractions.Workshop;
using Application.Identity;
using Common.Result;
using Common.Result.Fields;
using Domain.User;
using Domain.Workshop;
using eMechanic.Infrastructure.Identity;
using eMechanic.Infrastructure.Services.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

public class AuthenticatorTests
{
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IWorkshopRepository _workshopRepository;
    private readonly Authenticator _authenticator;

    private readonly Identity _fakeUserIdentity;
    private readonly Identity _fakeWorkshopIdentity;
    private readonly User _fakeUserDomain;
    private readonly Workshop _fakeWorkshopDomain;

    private const string TEST_USER_EMAIL = "test@user.com";
    private const string TEST_WORKSHOP_EMAIL = "test@workshop.com";
    private const string TEST_PASSWORD = "Password123!";

    public AuthenticatorTests()
    {
        var userStoreMock = Substitute.For<IUserStore<Identity>>();
        _userManager = Substitute.For<UserManager<Identity>>(userStoreMock, null, null, null, null, null, null, null, null);
        _userRepository = Substitute.For<IUserRepository>();
        _workshopRepository = Substitute.For<IWorkshopRepository>();

        _authenticator = new Authenticator(_userManager, _userRepository, _workshopRepository);

        _fakeUserIdentity = Identity.Create(TEST_USER_EMAIL, EIdentityType.User);
        _fakeUserIdentity.Id = Guid.NewGuid();
        _fakeUserDomain = User.Create(TEST_USER_EMAIL, "Test", "User", _fakeUserIdentity.Id);

        _fakeWorkshopIdentity = Identity.Create(TEST_WORKSHOP_EMAIL, EIdentityType.Workshop);
        _fakeWorkshopIdentity.Id = Guid.NewGuid();
        _fakeWorkshopDomain = Workshop.Create(
            "contact@workshop.com", TEST_WORKSHOP_EMAIL, "Display", "Workshop Name",
            "123", "Addr", "City", "PC", "PL", _fakeWorkshopIdentity.Id);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_ReturnInvalidCredentialsError_WhenEmailNotFound()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_USER_EMAIL).Returns(Task.FromResult<Identity?>(null));

        // Act
        var result = await _authenticator.AuthenticateAsync(TEST_USER_EMAIL, TEST_PASSWORD, EIdentityType.User);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors![EField.General.ToString()].Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task AuthenticateAsync_Should_ReturnInvalidCredentialsError_WhenPasswordIsIncorrect()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_USER_EMAIL).Returns(_fakeUserIdentity);
        _userManager.CheckPasswordAsync(_fakeUserIdentity, TEST_PASSWORD).Returns(false);

        // Act
        var result = await _authenticator.AuthenticateAsync(TEST_USER_EMAIL, TEST_PASSWORD, EIdentityType.User);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors![EField.General.ToString()].Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task AuthenticateAsync_Should_ReturnInvalidCredentialsError_WhenExpectedTypeIsWrong()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_USER_EMAIL).Returns(_fakeUserIdentity);

        // Act
        var result = await _authenticator.AuthenticateAsync(TEST_USER_EMAIL, TEST_PASSWORD, EIdentityType.Workshop);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors![EField.General.ToString()].Should().Contain("Invalid email or password.");

        await _userManager.DidNotReceiveWithAnyArgs().CheckPasswordAsync(default!, default!);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_ReturnInvalidCredentialsError_WhenDomainEntityIsMissing()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_USER_EMAIL).Returns(_fakeUserIdentity);
        _userManager.CheckPasswordAsync(_fakeUserIdentity, TEST_PASSWORD).Returns(true);

        _userRepository.GetByIdentityIdAsync(_fakeUserIdentity.Id).Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _authenticator.AuthenticateAsync(TEST_USER_EMAIL, TEST_PASSWORD, EIdentityType.User);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors![EField.General.ToString()].Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task AuthenticateAsync_Should_ReturnSuccess_ForValidUser()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_USER_EMAIL).Returns(_fakeUserIdentity);
        _userManager.CheckPasswordAsync(_fakeUserIdentity, TEST_PASSWORD).Returns(true);
        _userRepository.GetByIdentityIdAsync(_fakeUserIdentity.Id).Returns(_fakeUserDomain);

        // Act
        var result = await _authenticator.AuthenticateAsync(TEST_USER_EMAIL, TEST_PASSWORD, EIdentityType.User);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.IdentityId.Should().Be(_fakeUserIdentity.Id);
        result.Value.DomainEntityId.Should().Be(_fakeUserDomain.Id);
        result.Value.Type.Should().Be(EIdentityType.User);
    }

    [Fact]
    public async Task AuthenticateAsync_Should_ReturnSuccess_ForValidWorkshop()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_WORKSHOP_EMAIL).Returns(_fakeWorkshopIdentity);
        _userManager.CheckPasswordAsync(_fakeWorkshopIdentity, TEST_PASSWORD).Returns(true);
        _workshopRepository.GetByIdentityIdAsync(_fakeWorkshopIdentity.Id).Returns(_fakeWorkshopDomain);

        // Act
        var result = await _authenticator.AuthenticateAsync(TEST_WORKSHOP_EMAIL, TEST_PASSWORD, EIdentityType.Workshop);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.IdentityId.Should().Be(_fakeWorkshopIdentity.Id);
        result.Value.DomainEntityId.Should().Be(_fakeWorkshopDomain.Id);
        result.Value.Type.Should().Be(EIdentityType.Workshop);
    }
}
