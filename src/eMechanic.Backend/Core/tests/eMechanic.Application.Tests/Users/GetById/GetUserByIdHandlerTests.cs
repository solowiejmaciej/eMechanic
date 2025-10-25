namespace eMechanic.Application.Tests.Users.GetById;

using Domain.User;
using eMechanic.Application.Abstractions.User;
using eMechanic.Application.Users.GetById;
using eMechanic.Common.Result;
using NSubstitute;

public class GetUserByIdHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new GetUserByIdHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserResponse_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        var fakeUser = User.Create("test@user.pl", "Test", "User", Guid.NewGuid());

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                       .Returns(fakeUser);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.HasError());
        Assert.Equal(fakeUser.Id, result.Value!.Id);
        Assert.Equal(fakeUser.FirstName, result.Value.FirstName);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult<User?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.HasError());
        Assert.Equal(EErrorCode.NotFoundError, result.Error!.Code);
    }
}
