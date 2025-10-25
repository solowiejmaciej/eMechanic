namespace eMechanic.Application.Tests.Vehicle.Create;

using Application.Abstractions.Identity.Contexts;
using Application.Abstractions.Vehicle;
using Application.Vehicle.Create;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class CreateVehicleHandlerTests
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;
    private readonly CreateVehicleHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();

    public CreateVehicleHandlerTests()
    {
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new CreateVehicleHandler(_vehicleRepository, _userContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResultWithVehicleId_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "V1N123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        _vehicleRepository.AddAsync(Arg.Any<Vehicle>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Guid.NewGuid()));
        _userContext.UserId.Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        await _vehicleRepository.Received(1).AddAsync(Arg.Is<Vehicle>(v => v.UserId == _currentUserId && v.Vin.Value == command.Vin), Arg.Any<CancellationToken>());
        await _vehicleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorResult_WhenVehicleCreationFailsInDomain()
    {
        // Arrange
        var command = new CreateVehicleCommand(
            "INVALID", "Test Manufacturer", "Test Model", "2023",
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

        _userContext.UserId.Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
         var command = new CreateVehicleCommand(
            "V1N123456789ABCDE", "Test Manufacturer", "Test Model", "2023",
            1.6m, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);

         _userContext.UserId.ThrowsForAnyArgs<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
        await _vehicleRepository.DidNotReceiveWithAnyArgs().AddAsync(default!, default);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}
