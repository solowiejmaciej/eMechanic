namespace eMechanic.Application.Tests.UserPreferences.Features.Get;

using Domain.Tests.Builders;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.UserRepairPreferences.Features.Get;
using eMechanic.Common.Result;
using eMechanic.Domain.UserRepairPreferences;
using eMechanic.Domain.UserRepairPreferences.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using UserRepairPreferences.Repositories;

public class GetCurrentUserRepairPreferencesHandlerTests
{
    private readonly IUserRepairPreferencesRepository _preferencesRepository;
    private readonly IUserContext _userContext;
    private readonly GetCurrentUserRepairPreferencesHandler _handler;
    private readonly Guid _currentUserId = Guid.NewGuid();

    public GetCurrentUserRepairPreferencesHandlerTests()
    {
        _preferencesRepository = Substitute.For<IUserRepairPreferencesRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new GetCurrentUserRepairPreferencesHandler(_preferencesRepository, _userContext);

        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);
    }

    [Fact]
    public async Task Handle_Should_ReturnPreferences_WhenFoundForUser()
    {
        // Arrange
        var query = new GetCurrentUserRepairPreferencesQuery();
        var preferences = new UserRepairPreferencesBuilder().WithUserId(_currentUserId).Build();

        _preferencesRepository.GetByUserIdAsync(_currentUserId, Arg.Any<CancellationToken>())
            .Returns(preferences);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.UserId.Should().Be(_currentUserId);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenPreferencesNotFound()
    {
        // Arrange
        var query = new GetCurrentUserRepairPreferencesQuery();

        _preferencesRepository.GetByUserIdAsync(_currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<UserRepairPreferences?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var query = new GetCurrentUserRepairPreferencesQuery();
        _userContext.GetUserId().Throws(new UnauthorizedAccessException());

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
