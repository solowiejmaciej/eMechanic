namespace eMechanic.Infrastructure.Payments;

using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Common;
using eMechanic.Common.Result;
using eMechanic.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

internal sealed class StripePaymentWebhookProcessor : IPaymentWebhookProcessor
{
    private readonly IOptions<StripeOptions> _options;

    public StripePaymentWebhookProcessor(IOptions<StripeOptions> options)
    {
        _options = options;
    }

    public Task<Result<PayableItem, Error>> ProcessAsync(
        string jsonPayload,
        string signatureHeader,
        CancellationToken cancellationToken)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                jsonPayload,
                signatureHeader,
                _options.Value.WebhookSecret,
                throwOnApiVersionMismatch: false);

            if (stripeEvent.Type != "checkout.session.completed")
            {
                return Task.FromResult<Result<PayableItem, Error>>(
                    new Error(EErrorCode.ValidationError, $"Unsupported Stripe event type: '{stripeEvent.Type}'."));
            }

            if (stripeEvent.Data.Object is not Session session)
            {
                return Task.FromResult<Result<PayableItem, Error>>(
                    new Error(EErrorCode.InternalServerError, "Failed to deserialize Stripe session object."));
            }

            if (!session.Metadata.TryGetValue("reference_id", out var referenceIdRaw) ||
                !Guid.TryParse(referenceIdRaw, out var referenceId))
            {
                return Task.FromResult<Result<PayableItem, Error>>(
                    new Error(EErrorCode.ValidationError, "Missing or invalid 'reference_id' metadata."));
            }

            if (!session.Metadata.TryGetValue("payable_type", out var payableTypeRaw) ||
                !Enum.TryParse<EPayableType>(payableTypeRaw, out var payableType))
            {
                return Task.FromResult<Result<PayableItem, Error>>(
                    new Error(EErrorCode.ValidationError, "Missing or invalid 'payable_type' metadata."));
            }

            if (!session.Metadata.TryGetValue("payer_id", out var payerIdRaw) ||
                !Guid.TryParse(payerIdRaw, out var payerId))
            {
                return Task.FromResult<Result<PayableItem, Error>>(
                    new Error(EErrorCode.ValidationError, "Missing or invalid 'payer_id' metadata."));
            }

            var amountInUnits = (session.AmountTotal ?? 0) / 100m;
            var currency = (session.Currency ?? "pln").ToUpperInvariant();

            var moneyResult = Money.Create(amountInUnits, currency);

            if (moneyResult.HasError())
            {
                return Task.FromResult<Result<PayableItem, Error>>(moneyResult.Error!);
            }

            var payableItem = new PayableItem(referenceId, payableType, moneyResult.Value!, payerId);

            return Task.FromResult<Result<PayableItem, Error>>(payableItem);
        }
        catch (StripeException ex)
        {
            return Task.FromResult<Result<PayableItem, Error>>(
                new Error(EErrorCode.ValidationError, $"Stripe signature verification failed: {ex.Message}"));
        }
    }
}



