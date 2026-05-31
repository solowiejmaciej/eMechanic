namespace eMechanic.Integration.Tests.Payment;

using Application.Payments.Abstractions;
using Application.Payments.Common;
using Common.Result;
using Domain.Shared.ValueObjects;

public sealed class MockPaymentWebhookProcessor : IPaymentWebhookProcessor
{
    public PayableItem? PayableItemToReturn { get; set; }

    public Task<Result<PayableItem, Error>> ProcessAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken)
    {
        if (PayableItemToReturn is null)
        {
            return Task.FromResult<Result<PayableItem, Error>>(
                new Error(EErrorCode.ValidationError, "Mock processor: no payable item configured."));
        }

        return Task.FromResult<Result<PayableItem, Error>>(PayableItemToReturn);
    }

    public void Configure(Guid referenceId, EPayableType type, decimal amount = 2000m, string currency = "PLN")
    {
        var money = Money.Create(amount, currency).Value!;
        PayableItemToReturn = new PayableItem(referenceId, type, money, Guid.NewGuid());
    }
}

