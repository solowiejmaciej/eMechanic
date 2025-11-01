namespace eMechanic.Application.Tests.Vehicle.Get.GetById;

using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Abstractions.Vehicle;
using eMechanic.Application.Vehicle.Get.ById;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class GetVehicleByIdHandlerTests
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;
    private readonly GetVehicleByIdHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _existingVehicle;

    public GetVehicleByIdHandlerTests()
    {
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _userContext = Substitute.For<IUserContext>();
        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);

         var creationResult = Vehicle.Create(
            _currentUserId, "V1N123456789ABCDE", "GetById", "ModelGet", "2022",
            3.0m, EFuelType.Hybrid, EBodyType.Hatchback, EVehicleType.Passenger);
        creationResult.HasError().Should().BeFalse();
        _existingVehicle = creationResult.Value!;
        typeof(Vehicle).GetProperty("Id")!.SetValue(_existingVehicle, _vehicleId);

        _handler = new GetVehicleByIdHandler(_vehicleRepository, _userContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnVehicleResponse_WhenVehicleExistsForUser()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(_vehicleId);

         _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Vehicle?>(_existingVehicle));

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
    public async Task Handle_Should_ReturnNotFoundError_WhenVehicleDoesNotExistForUser()
    {
        // Arrange
        var query = new GetVehicleByIdQuery(_vehicleId);

        _vehicleRepository.GetForUserById(_vehicleId, _currentUserId, Arg.Any<CancellationToken>())
             .Returns(Task.FromResult<Vehicle?>(null));

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
        var query = new GetVehicleByIdQuery(_vehicleId);
        _userContext.GetUserId().ThrowsForAnyArgs<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserById(default, default, default);
    }
}
