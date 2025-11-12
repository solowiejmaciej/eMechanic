namespace eMechanic.Application.Tests.Workshop.Features.Create;

using Application.Tests.Builders;
using Application.Workshop.Features.Create;
using eMechanic.Application.Workshop.Services;
using eMechanic.Common.Result;
using NSubstitute;

public class CreateWorkshopHandlerTests
{
    private readonly IWorkshopService _workshopService;
    private readonly CreateWorkshopHandler _handler;

    public CreateWorkshopHandlerTests()
    {
        _workshopService = Substitute.For<IWorkshopService>();
        _handler = new CreateWorkshopHandler(_workshopService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenWorkshopIsCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateWorkshopCommandBuilder().Build();

        var newWorkshopId = Guid.NewGuid();

        _workshopService.CreateWorkshopWithIdentityAsync(
            command.Email, command.Password, command.ContactEmail,
            command.Name, command.DisplayName, command.PhoneNumber,
            command.Address, command.City, command.PostalCode,
            command.Country, Arg.Any<CancellationToken>())
        .Returns(newWorkshopId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.HasError());
        Assert.Equal(newWorkshopId, result.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorResult_WhenCreatorServiceFails()
    {
        // Arrange
        var command = new CreateWorkshopCommandBuilder().Build();

        var error = new Error(EErrorCode.ValidationError, "Identity with given email already exists.");

        _workshopService.CreateWorkshopWithIdentityAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(error);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.HasError());
        Assert.Equal(error, result.Error);
    }
}
