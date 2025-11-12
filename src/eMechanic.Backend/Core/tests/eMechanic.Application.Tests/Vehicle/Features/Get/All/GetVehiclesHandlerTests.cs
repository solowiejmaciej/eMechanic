namespace eMechanic.Application.Tests.Vehicle.Features.Get.All;

using Application.Vehicle.Repostories;
using Domain.Tests.Builders;
using eMechanic.Application.Abstractions.Identity.Contexts;
using eMechanic.Application.Vehicle.Features.Get.All;
using eMechanic.Common.Result;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.Vehicle.Enums;
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

    [Fact]
    public async Task Handle_Should_ReturnPaginatedResult_WhenVehiclesExistForUser()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetVehiclesQuery(parameters);

        var vehicles = new List<Vehicle>
        {
            new VehicleBuilder().WithVin("V1N123456789ABCDE").WithManufacturer("ManufacturerA").Build(),
            new VehicleBuilder().WithVin("V1N123456789ABCDF").WithManufacturer("ManufacturerB").Build(),
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
