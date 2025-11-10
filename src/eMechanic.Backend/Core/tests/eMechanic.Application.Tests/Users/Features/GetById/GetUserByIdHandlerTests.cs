namespace eMechanic.Application.Tests.Users.Features.GetById;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.User;
using eMechanic.Application.Users.Features.Get.Current;
using eMechanic.Common.Result;
using eMechanic.Domain.User;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class GetCurrentUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly GetCurrentUserHandler _handler;
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly User _fakeUser;

    public GetCurrentUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new GetCurrentUserHandler(_userRepository, _userContext);

        _fakeUser = User.Create("test@user.pl", "Test", "User", Guid.NewGuid());
        typeof(User).GetProperty("Id")!.SetValue(_fakeUser, _currentUserId);

        // Konfiguracja mocka UserContext
        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserResponse_WhenUserIsAuthenticated()
    {
        // Arrange
        var query = new GetCurrentUserQuery();
        _userRepository.GetByIdAsync(_currentUserId, Arg.Any<CancellationToken>())
                       .Returns(_fakeUser);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(_currentUserId);
        result.Value.Email.Should().Be(_fakeUser.Email);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenUserFromTokenIsNotFoundInDb()
    {
        // Arrange
        var query = new GetCurrentUserQuery();
        _userRepository.GetByIdAsync(_currentUserId, Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_Throw_WhenUserContextThrowsUnauthorized()
    {
        // Arrange
        var query = new GetCurrentUserQuery();
        _userContext.GetUserId().Throws<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
