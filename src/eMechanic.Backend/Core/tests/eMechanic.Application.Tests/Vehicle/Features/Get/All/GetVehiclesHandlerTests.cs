namespace eMechanic.Application.Tests.Vehicle.Get.All;

using Application.Abstractions.Identity.Contexts;
using Application.Abstractions.Vehicle;
using Application.Vehicle.Features.Get.All;
using Common.Result;
using Domain.Vehicle;
using Domain.Vehicle.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class GetVehiclesHandlerTests
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserContext _userContext;
    private readonly GetVehiclesHandler _handler;

    private readonly Guid _currentUserId = Guid.NewGuid();

    public GetVehiclesHandlerTests()
    {
        _vehicleRepository = Substitute.For<IVehicleRepository>();
        _userContext = Substitute.For<IUserContext>();
        _userContext.GetUserId().Returns(_currentUserId);
        _userContext.IsAuthenticated.Returns(true);

        _handler = new GetVehiclesHandler(_vehicleRepository, _userContext);
    }

    private Vehicle CreateTestVehicle(string vin, string manufacturer)
    {
        var result = Vehicle.Create(
            _currentUserId, vin, manufacturer, "TestModel", "2022",
            1.5m,  200, EMileageUnit.Miles, EFuelType.Gasoline, EBodyType.Sedan, EVehicleType.Passenger);
        result.HasError().Should().BeFalse();
        return result.Value!;
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedResult_WhenVehiclesExistForUser()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehiclesQuery(parameters);

        var vehicles = new List<Vehicle>
        {
            CreateTestVehicle("V1N123456789ABCDE", "ManufacturerA"),
            CreateTestVehicle("V1N123456789ABCDF", "ManufacturerB")
        };
        var paginatedResult = new PaginationResult<Vehicle>(vehicles, 2, 1, 10);

        _vehicleRepository.GetForUserPaginatedAsync(parameters, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(paginatedResult));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Items.Count().Should().Be(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.PageNumber.Should().Be(1);
        result.Value.Items.First().Manufacturer.Should().Be("ManufacturerA");

        await _vehicleRepository.Received(1).GetForUserPaginatedAsync(parameters, _currentUserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyResult_WhenNoVehiclesExistForUser()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehiclesQuery(parameters);

        var emptyPaginatedResult = new PaginationResult<Vehicle>(new List<Vehicle>(), 0, 1, 10);

        _vehicleRepository.GetForUserPaginatedAsync(parameters, _currentUserId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(emptyPaginatedResult));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_ThrowUnauthorizedAccessException_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehiclesQuery(parameters);
        _userContext.GetUserId().ThrowsForAnyArgs<UnauthorizedAccessException>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(query, CancellationToken.None));
        await _vehicleRepository.DidNotReceiveWithAnyArgs().GetForUserPaginatedAsync(default!, default, default);
    }
}
