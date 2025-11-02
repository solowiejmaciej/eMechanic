namespace eMechanic.Infrastructure.Tests.Services.Creators;

using Application.Abstractions.Identity;
using Application.Abstractions.User;
using Application.Identity;
using Common.Result;
using Common.Result.Fields;
using Domain.User;
using eMechanic.Infrastructure.Identity;
using eMechanic.Infrastructure.Services.Creators;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class UserCreatorServiceTests
{
    private readonly UserManager<Identity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionalExecutor _transactionalExecutor;
    private readonly ILogger<UserCreatorService> _logger;
    private readonly UserCreatorService _userCreatorService;

    private const string TEST_EMAIL = "test@user.com";
    private const string TEST_PASSWORD = "Password123!";
    private const string TEST_FIRST_NAME = "Test";
    private const string TEST_LAST_NAME = "User";

    public UserCreatorServiceTests()
    {
        var userStoreMock = Substitute.For<IUserStore<Identity>>();
        _userManager = Substitute.For<UserManager<Identity>>(userStoreMock, null, null, null, null, null, null, null, null);

        _userRepository = Substitute.For<IUserRepository>();
        _transactionalExecutor = Substitute.For<ITransactionalExecutor>();
        _logger = Substitute.For<ILogger<UserCreatorService>>();

        _userCreatorService = new UserCreatorService(
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
    }

    [Fact]
    public async Task CreateUserWithIdentityAsync_Should_ReturnEmailExistsError_WhenIdentityAlreadyExists()
    {
        // Arrange
        var existingIdentity = Identity.Create(TEST_EMAIL, EIdentityType.User);
        _userManager.FindByEmailAsync(TEST_EMAIL).Returns(existingIdentity);

        // Act
        var result = await _userCreatorService.CreateUserWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_FIRST_NAME, TEST_LAST_NAME, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        result.Error.ValidationErrors.Should().ContainKey(EField.Email.ToString());
        result.Error.ValidationErrors![EField.Email.ToString()].Should().Contain("Identity with given email already exists.");

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
        var result = await _userCreatorService.CreateUserWithIdentityAsync(
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
        var result = await _userCreatorService.CreateUserWithIdentityAsync(
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
        var result = await _userCreatorService.CreateUserWithIdentityAsync(
            TEST_EMAIL, TEST_PASSWORD, TEST_FIRST_NAME, TEST_LAST_NAME, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeEmpty();

        await _transactionalExecutor.Received(1).ExecuteAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>());
        await _userManager.Received(1).CreateAsync(Arg.Is<Identity>(i => i.Email == TEST_EMAIL), TEST_PASSWORD);
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u => u.Email == TEST_EMAIL && u.FirstName == TEST_FIRST_NAME), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
