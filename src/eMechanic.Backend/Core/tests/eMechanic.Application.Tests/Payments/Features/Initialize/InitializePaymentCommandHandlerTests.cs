namespace eMechanic.Application.Tests.Payments.Features.Initialize;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Application.Payments.Features.Initialize;
using Application.Repair.Repositories;
using Application.Vehicle.Vehicle.Services;
using Common.Result;
using Domain.Repair.Enums;
using Domain.Tests.Builders;
using FluentAssertions;
using NSubstitute;

public class InitializePaymentCommandHandlerTests
{
    private readonly IRepairRepository _repairRepository = Substitute.For<IRepairRepository>();
    private readonly IVehicleOwnershipService _vehicleOwnershipService = Substitute.For<IVehicleOwnershipService>();
    private readonly IPaymentService _paymentService = Substitute.For<IPaymentService>();
    private readonly InitializePaymentCommandHandler _handler;

    public InitializePaymentCommandHandlerTests()
    {
        _handler = new InitializePaymentCommandHandler(
            _repairRepository,
            _vehicleOwnershipService,
            _paymentService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSessionDto_WhenRepairIsCompleted()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var vehicle = new VehicleBuilder().WithOwnerId(ownerId).Build();
        var repair = new RepairBuilder()
            .WithVehicleId(vehicle.Id)
            .WithStatus(ERepairStatus.Completed)
            .Build();

        var expectedSession = new PaymentSessionDto("sess_123", "https://checkout.stripe.com/pay/sess_123");

        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>())
            .Returns(repair);

        _vehicleOwnershipService
            .GetAndVerifyOwnershipAsync(repair.VehicleId, Arg.Any<CancellationToken>())
            .Returns(vehicle);

        _paymentService.CreateCheckoutSessionAsync(
                Arg.Any<PayableItem>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedSession);

        var command = new InitializePaymentCommand(
            repair.Id, EPayableType.Repair,
            "https://success.example.com", "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.SessionId.Should().Be("sess_123");
        result.Value.CheckoutUrl.Should().Be("https://checkout.stripe.com/pay/sess_123");

        await _paymentService.Received(1).CreateCheckoutSessionAsync(
            Arg.Is<PayableItem>(p => p.ReferenceId == repair.Id && p.PayerId == ownerId),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenRepairDoesNotExist()
    {
        // Arrange
        _repairRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Domain.Repair.Repair?)null);

        var command = new InitializePaymentCommand(
            Guid.NewGuid(), EPayableType.Repair,
            "https://success.example.com", "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.NotFoundError);
        await _paymentService.DidNotReceive()
            .CreateCheckoutSessionAsync(Arg.Any<PayableItem>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenRepairIsNotCompleted()
    {
        // Arrange
        var repair = new RepairBuilder()
            .WithStatus(ERepairStatus.InProgress)
            .Build();

        _repairRepository.GetByIdAsync(repair.Id, Arg.Any<CancellationToken>())
            .Returns(repair);

        var command = new InitializePaymentCommand(
            repair.Id, EPayableType.Repair,
            "https://success.example.com", "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        await _vehicleOwnershipService.DidNotReceive()
            .GetAndVerifyOwnershipAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationError_WhenTypeIsNotSupported()
    {
        // Arrange
        var command = new InitializePaymentCommand(
            Guid.NewGuid(), EPayableType.Subscription,
            "https://success.example.com", "https://cancel.example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be(EErrorCode.ValidationError);
        await _repairRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
