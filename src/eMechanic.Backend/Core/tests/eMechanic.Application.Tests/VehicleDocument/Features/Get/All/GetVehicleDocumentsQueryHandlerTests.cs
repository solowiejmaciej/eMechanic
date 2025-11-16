namespace eMechanic.Application.Tests.VehicleDocument.Features.Get.All;

using System.Threading;
using System.Threading.Tasks;
using eMechanic.Application.Tests.Builders;
using eMechanic.Application.Vehicle.Services;
using eMechanic.Application.VehicleDocument.Features.Get.All;
using eMechanic.Application.VehicleDocument.Repositories;
using eMechanic.Common.Result;
using eMechanic.Domain.Tests.Builders;
using eMechanic.Domain.Vehicle;
using eMechanic.Domain.VehicleDocument;
using FluentAssertions;
using NSubstitute;

public class GetVehicleDocumentsQueryHandlerTests
{
    private readonly IVehicleOwnershipService _ownershipService;
    private readonly IVehicleDocumentRepository _documentRepository;
    private readonly GetVehicleDocumentsQueryHandler _handler;

    private readonly Guid _vehicleId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly GetVehicleDocumentsQuery _query;
    private readonly PaginationParameters _pagination = new() { PageNumber = 1, PageSize = 10 };

    public GetVehicleDocumentsQueryHandlerTests()
    {
        _ownershipService = Substitute.For<IVehicleOwnershipService>();
        _documentRepository = Substitute.For<IVehicleDocumentRepository>();
        _handler = new GetVehicleDocumentsQueryHandler(_ownershipService, _documentRepository);

        var vehicle = new VehicleBuilder().WithOwnerId(_userId).Build();
        _query = new GetVehicleDocumentsQueryBuilder().WithVehicleId(_vehicleId).Build();

        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Result<Vehicle, Error>>(vehicle));
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedDocuments_WhenOwnerIsVerified()
    {
        // Arrange
        var documents = new List<VehicleDocument> { new VehicleDocumentBuilder().Build() };

        var paginatedResult = new PaginationResult<VehicleDocument>(
            documents,
            1,
            _pagination.PageNumber,
            _pagination.PageSize);

        _documentRepository.GetByVehicleIdPaginatedAsync(
                _vehicleId,
                Arg.Any<PaginationParameters>(),
                Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Items.Count().Should().Be(1);
        result.Value.TotalCount.Should().Be(1);
        result.Value.Items.First().OriginalFileName.Should().Be("test-faktura.pdf");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenOwnershipFails()
    {
        // Arrange
        var error = new Error(EErrorCode.NotFoundError, "Not owner");
        _ownershipService.GetAndVerifyOwnershipAsync(_vehicleId, Arg.Any<CancellationToken>())
            .Returns(error);

        // Act
        var result = await _handler.Handle(_query, CancellationToken.None);

        // Assert
        result.HasError().Should().BeTrue();
        result.Error.Should().Be(error);
        await _documentRepository.DidNotReceiveWithAnyArgs().GetByVehicleIdPaginatedAsync(default, default!, default);
    }
}
