namespace eMechanic.Integration.Tests.Payment;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Common.Result;
using Domain.Payment.Enums;
using Domain.Shared.ValueObjects;

public sealed class MockPaymentProcessor : IPaymentProcessor
{
    public PayableItem? PayableItemToReturn { get; set; }
    public string ProviderSessionId { get; set; } = "cs_test_mock_session";

    public Task<Result<PaymentSessionDto, Error>> CreateCheckoutSessionAsync(
        eMechanic.Domain.Payment.PaymentOrder paymentOrder,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<PaymentSessionDto, Error>>(
            new PaymentSessionDto(ProviderSessionId, $"https://checkout.mock/{ProviderSessionId}"));
    }

    public Task<Result<PaymentProcessorPayload, Error>> ProcessAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken)
    {
        if (PayableItemToReturn is null)
        {
            return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                new Error(EErrorCode.ValidationError, "Mock processor: no payable item configured."));
        }

        return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
            new PaymentProcessorPayload(PayableItemToReturn.ReferenceId, PayableItemToReturn.Type, ProviderSessionId));
    }

    public void Configure(Guid referenceId, EPayableType type, string providerSessionId, decimal amount = 2000m, string currency = "PLN")
    {
        ProviderSessionId = providerSessionId;
        var money = Money.Create(amount, currency).Value!;
        PayableItemToReturn = new PayableItem(referenceId, type, money, Guid.NewGuid());
    }
}
