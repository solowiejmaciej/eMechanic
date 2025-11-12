namespace eMechanic.Application.Tests.Vehicle.Features.Get.GetById;

using Application.Vehicle.Services;
using Domain.Tests.Builders;
using eMechanic.Application.Vehicle.Features.Get.ById;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;

public class GetVehicleByIdHandlerTests
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly GetVehicleByIdHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _existingVehicle;

    public GetVehicleByIdHandlerTests()
    {
        _ownershipService = Substitute.For<IVehicleOwnershipService>();
        var creationResult = new VehicleBuilder().WithOwnerId(_currentUserId).BuildResult();

        creationResult.HasError().Should().BeFalse();
        _existingVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_existingVehicle, _vehicleId);

        _handler = new GetVehicleByIdHandler(_ownershipService);
    }

    [Fact]
    public async Task Handle_Should_ReturnVehicleResponse_WhenVehicleExistsForUser()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(_vehicleId);

         _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(_existingVehicle));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_vehicleId);
        result.Value.UserId.Should().Be(_currentUserId);
        result.Value.Vin.Should().Be(_existingVehicle.Vin.Value);
        result.Value.Manufacturer.Should().Be(_existingVehicle.Manufacturer.Value);
    }

     [Fact]
    public async Task Handle_Should_ReturnNotFoundError_WhenOwnershipServiceFails()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(_vehicleId);

        var notFoundError = new Error(EErrorCode.NotFoundError, "Vehicle not found.");
        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
             .Returns(Task.FromResult<Result<Vehicle, Error>>(notFoundError));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
    }

     [Fact]
    public async Task Handle_Should_ReturnUnauthorizedError_WhenOwnershipServiceReturnsUnauthorized()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(_vehicleId);

        var unauthorizedError = new Error(EErrorCode.UnauthorizedError, "User is not authenticated.");
        _ownershipService.GetAndVerifyOwnershipAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(unauthorizedError));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error!.Code.Should().Be(EErrorCode.UnauthorizedError);
    }
}
