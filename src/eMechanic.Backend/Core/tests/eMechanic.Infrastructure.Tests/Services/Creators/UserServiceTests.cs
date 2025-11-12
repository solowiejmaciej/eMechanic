namespace eMechanic.Infrastructure.Tests.Services.Creators;

using Application.Abstractions.Identity;
using Application.Identity;
using Common.Result;
using Common.Result.Fields;
using Domain.User;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Infrastructure.Identity;
using eMechanic.Infrastructure.Services.Creators;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Repositories;

public class UserServiceTests
{
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<UserService> _logger;
    private readonly UserService _userService;

    private const string TEST_EMAIL = "test@user.com";
    private const string TEST_PASSWORD = "Password123!";
    private const string TEST_FIRST_NAME = "Test";
    private const string TEST_LAST_NAME = "User";

    private readonly Guid _domainUserId = Guid.NewGuid();
    private readonly Guid _identityId = Guid.NewGuid();
    private readonly User _fakeUser;
    private readonly Identity _fakeIdentity;

    public UserServiceTests()
    {
        var userStoreMock = Substitute.For<IUserStore<Identity>>();
        _userManager = Substitute.For<UserManager<Identity>>(userStoreMock, null, null, null, null, null, null, null, null);

        _userRepository = Substitute.For<IUserRepository>();
        _transactionalExecutor = Substitute.For<ITransactionalExecutor>();
        _logger = Substitute.For<ILogger<UserService>>();

        _userService = new UserService(
            _userManager,
            _userRepository,
            _transactionalExecutor,
            _logger);

        _transactionalExecutor
            .ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>())
            .Returns(async call =>
            {
                var operation = call.Arg<Func<Task>>();
                await operation();
            });

        _fakeUser = new UserBuilder().WithEmail(TEST_EMAIL).WithFirstName(TEST_FIRST_NAME).WithLastName(TEST_LAST_NAME).WithIdentityId(_identityId).Build();
        _fakeIdentity = Identity.Create(TEST_EMAIL, EIdentityType.User);
        _fakeIdentity.Id = _identityId;
    }

    [Fact]
    public async Task CreateUserWithIdentityAsync_Should_ReturnEmailExistsError_WhenIdentityAlreadyExists()
    {
        // Arrange
        var existingIdentity = Identity.Create(TEST_EMAIL, EIdentityType.User);
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(existingIdentity);

        // Act
        var result = await _userService.CreateUserWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_FIRST_NAME, TEST_LAST_NAME, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors.Should().ContainKey(nameof(EField.Email));
        result.Error.ValidationErrors![nameof(EField.Email)].Should().Contain("Identity with given email already exists.");

        await _transactionalExecutor.DidNotReceiveWithAnyArgs().ExecuteAsync(default!, default);
    }

    [Fact]
    public async Task CreateUserWithIdentityAsync_Should_ReturnValidationError_WhenIdentityCreationFails()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(Task.FromResult<Identity?>(null));

        var identityErrors = new[] { new IdentityError { Code = "TestError", Description = "Password too weak." } };

        _userManager.CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD)
            .Returns(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _userService.CreateUserWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_FIRST_NAME, TEST_LAST_NAME, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors.Should().ContainKey("IdentityErrors");
        result.Error.ValidationErrors!["IdentityErrors"].Should().Contain("Password too weak.");

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
    }

    [Fact]
    public async Task CreateUserWithIdentityAsync_Should_ReturnInternalError_WhenDomainUserCreationFails()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(Task.FromResult<Identity?>(null));

        _userManager.CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD)
            .Returns(IdentityResult.Success);

        _userRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act
        var result = await _userService.CreateUserWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_FIRST_NAME, TEST_LAST_NAME, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.InternalServerError);
    }

    [Fact]
    public async Task CreateUserWithIdentityAsync_Should_ReturnSuccess_WhenCreationIsSuccessful()
    {
        // Arrange
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(Task.FromResult<Identity?>(null));

        _userManager.CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD)
            .Returns(call =>
            {
                var identity = call.Arg<Identity>();
                identity.Id = Guid.NewGuid();
                return IdentityResult.Success;
            });

        // Act
        var result = await _userService.CreateUserWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_FIRST_NAME, TEST_LAST_NAME, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userManager.Received(1).CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD);
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u => u.Email == TEST_EMAIL && u.FirstName == TEST_FIRST_NAME), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task UpdateUserWithIdentityAsync_Should_ReturnSuccess_WhenDataIsValidAndEmailIsUnchanged()
    {
        // Arrange
        var newFirstName = "UpdatedName";
        var newLastName = "UpdatedLastName";

        _userRepository.GetByIdAsync(_domainUserId, Arg.Any<CancellationToken>()).Returns(_fakeUser);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);

        // Act
        var result = await _userService.UpdateUserWithIdentityAsync(
            _domainUserId,
            TEST_EMAIL,
            newFirstName,
            newLastName,
            CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());

        await _userManager.DidNotReceiveWithAnyArgs().SetEmailAsync(default!, default!);
        await _userManager.DidNotReceiveWithAnyArgs().SetUserNameAsync(default!, default!);

        _userRepository.Received(1).UpdateAsync(Arg.Is<User>(u =>
            u.FirstName == newFirstName &&
            u.LastName == newLastName &&
            u.Email == TEST_EMAIL
        ), Arg.Any<CancellationToken>());

        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUserWithIdentityAsync_Should_ReturnSuccess_WhenDataIsValidAndEmailIsChanged()
    {
        // Arrange
        var newEmail = "new-email@test.com";
        var newFirstName = "UpdatedName";
        var newLastName = "UpdatedLastName";

        _userRepository.GetByIdAsync(_domainUserId, Arg.Any<CancellationToken>()).Returns(_fakeUser);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);

        _userManager.FindByEmailAsync(newEmail).Returns(Task.FromResult<Identity?>(null));
        _userManager.SetEmailAsync(_fakeIdentity, newEmail).Returns(IdentityResult.Success);
        _userManager.SetUserNameAsync(_fakeIdentity, newEmail).Returns(IdentityResult.Success);

        // Act
        var result = await _userService.UpdateUserWithIdentityAsync(
            _domainUserId,
            newEmail,
            newFirstName,
            newLastName,
            CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());

        await _userManager.Received(1).SetEmailAsync(_fakeIdentity, newEmail);
        await _userManager.Received(1).SetUserNameAsync(_fakeIdentity, newEmail);

        _userRepository.Received(1).UpdateAsync(Arg.Is<User>(u => u.Email == newEmail), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUserWithIdentityAsync_Should_ReturnNotFoundError_WhenDomainUserIsMissing()
    {
        // Arrange
        _userRepository.GetByIdAsync(_domainUserId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _userService.UpdateUserWithIdentityAsync(
            _domainUserId,
            "new@email.com",
            "New",
            "User",
            CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task UpdateUserWithIdentityAsync_Should_ReturnValidationError_WhenNewEmailIsTaken()
    {
        // Arrange
        var newEmail = "taken@email.com";
        var otherIdentity = Identity.Create(newEmail, EIdentityType.User);
        otherIdentity.Id = Guid.NewGuid();

        _userRepository.GetByIdAsync(_domainUserId, Arg.Any<CancellationToken>()).Returns(_fakeUser);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);

        _userManager.FindByEmailAsync(newEmail).Returns(otherIdentity);

        // Act
        var result = await _userService.UpdateUserWithIdentityAsync(
            _domainUserId,
            newEmail,
            "New",
            "User",
            CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors![EField.Email.ToString()].Should().Contain("Email already in use.");

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task UpdateUserWithIdentityAsync_Should_ReturnValidationError_WhenIdentityUpdateFails()
    {
        // Arrange
        var newEmail = "new-email@test.com";
        var identityErrors = new[] { new IdentityError { Code = "TestError", Description = "Invalid email format." } };

        _userRepository.GetByIdAsync(_domainUserId, Arg.Any<CancellationToken>()).Returns(_fakeUser);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);
        _userManager.FindByEmailAsync(newEmail).Returns(Task.FromResult<Identity?>(null));

        _userManager.SetEmailAsync(_fakeIdentity, newEmail).Returns(IdentityResult.Failed(identityErrors));
        _userManager.SetUserNameAsync(_fakeIdentity, newEmail).Returns(IdentityResult.Success);

        // Act
        var result = await _userService.UpdateUserWithIdentityAsync(
            _domainUserId,
            newEmail,
            "New",
            "User",
            CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors!["IdentityErrors"].Should().Contain("Invalid email format.");

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task UpdateUserWithIdentityAsync_Should_ReturnValidationError_WhenDomainUpdateThrowsArgumentException()
    {
        // Arrange
        var invalidFirstName = "";

        _userRepository.GetByIdAsync(_domainUserId, Arg.Any<CancellationToken>()).Returns(_fakeUser);
        _userManager.FindByIdAsync(_identityId.ToString()).Returns(_fakeIdentity);

        // Act
        var result = await _userService.UpdateUserWithIdentityAsync(
            _domainUserId,
            TEST_EMAIL,
            invalidFirstName,
            "NewLastName",
            CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.Message.Should().Contain("First name cannot be empty.");

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}
