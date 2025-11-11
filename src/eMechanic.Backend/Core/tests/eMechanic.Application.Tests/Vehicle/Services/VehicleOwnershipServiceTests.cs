namespace eMechanic.Application.Tests.Vehicle.Services;

using Application.Abstractions.Identity.Contexts;
using Application.Vehicle.Repostories;
using Application.Vehicle.Services;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class VehicleOwnershipServiceTests
{
    private readonly IUserContext _userContext;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly VehicleOwnershipService _service;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _fakeVehicle;

    public VehicleOwnershipServiceTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _service = new VehicleOwnershipService(_userContext, _vehicleRepository);

        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);

        var creationResult = Vehicle.Create(
            _currentUserId,
            "V1N123456789ABCDE",
            "Test",
            "Model",
            "2020",
            1.8m,
            10000,
            EMileageUnit.Kilometers,
            "PZ1W924",
            124,
            EFuelType.Gasoline,
            EBodyType.Sedan,
            EVehicleType.Passenger);

        creationResult.HasError().Should().BeFalse();
        _fakeVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_fakeVehicle, _vehicleId);
    }

    [Fact]
    public async Task GetAndVerifyOwnershipAsync_Should_ReturnVehicle_WhenUserIsOwner()
    {
        // Arrange
        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(_fakeVehicle));

        // Act
        var result = await _service.GetAndVerifyOwnershipAsync(_vehicleId, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().Be(_fakeVehicle);
        await _vehicleRepository.Received(1).GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAndVerifyOwnershipAsync_Should_ReturnNotFoundError_WhenVehicleNotFoundForUser()
    {
        // Arrange
        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(null));

        // Act
        var result = await _service.GetAndVerifyOwnershipAsync(_vehicleId, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        result.Error.Message.Should().Contain($"Vehicle with Id '{_vehicleId}' not found.");
    }

    [Fact]
    public async Task GetAndVerifyOwnershipAsync_Should_ReturnUnauthorizedError_WhenUserContextThrows()
    {
        // Arrange
        var unauthorizedException = new UnauthorizedAccessException("User is not authenticated.");
        _userContext.GetUserId().Throws(unauthorizedException);

        // Act
        var result = await _service.GetAndVerifyOwnershipAsync(_vehicleId, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
        result.Error.Message.Should().Be(unauthorizedException.Message);
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserById(default, default, default);
    }
}
