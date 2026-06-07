namespace eMechanic.Application.Tests.Vehicle.Services;

using Application.Abstractions.Identity.Contexts;
using Application.Vehicle.Vehicle.Repositories;
using Application.Vehicle.Vehicle.Services;
using Application.RepairRequest.Repositories;
using Application.Repair.Repositories;
using Common.Result;
using Domain.Tests.Builders;
using Domain.Vehicle;
using Domain.Vehicle.Vehicle;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.Generic;

public class VehicleOwnershipServiceTests
{
    private readonly IUserContext _userContext;
    private readonly IWorkshopContext _workshopContext;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IRepairRequestRepository _repairRequestRepository;
    private readonly IRepairRepository _repairRepository;
    private readonly VehicleOwnershipService _service;

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Vehicle _fakeVehicle;

    public VehicleOwnershipServiceTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _workshopContext = Substitute.For<IWorkshopContext>();
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _repairRequestRepository = Substitute.For<IRepairRequestRepository>();
        _repairRepository = Substitute.For<IRepairRepository>();
        
        _service = new VehicleOwnershipService(
            _userContext, 
            _workshopContext, 
            _vehicleRepository,
            _repairRequestRepository,
            _repairRepository);

        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);
        _workshopContext.IsAuthenticated.Returns(false);

        _repairRequestRepository.GetForWorkshopAsync(Arg.Any<Guid>(), Arg.Any<PaginationParameters>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PaginationResult<eMechanic.Domain.RepairRequest.RepairRequest>(new List<eMechanic.Domain.RepairRequest.RepairRequest>(), 0, 1, 100)));

        _repairRepository.GetForWorkshopPaginatedAsync(Arg.Any<Guid>(), Arg.Any<PaginationParameters>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PaginationResult<eMechanic.Domain.Repair.Repair>(new List<eMechanic.Domain.Repair.Repair>(), 0, 1, 100)));

        var creationResult = new VehicleBuilder().WithOwnerId(_currentUserId).BuildResult();

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
