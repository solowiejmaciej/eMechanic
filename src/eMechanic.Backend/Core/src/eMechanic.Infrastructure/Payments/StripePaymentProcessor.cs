namespace eMechanic.Infrastructure.Payments;

using Domain.Payment.Enums;
using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Common;
using eMechanic.Common.Result;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

internal sealed class StripePaymentProcessor : IPaymentProcessor
{
    private readonly IOptions<StripeOptions> _options;
    private readonly IStripePaymentService _stripePaymentService;

    public StripePaymentProcessor(
        IOptions<StripeOptions> options,
        IStripePaymentService stripePaymentService)
    {
        _options = options;
        _stripePaymentService = stripePaymentService;
    }

    public Task<Result<PaymentSessionDto, Error>> CreateCheckoutSessionAsync(
        eMechanic.Domain.Payment.PaymentOrder paymentOrder,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken)
    {
        return _stripePaymentService.CreateCheckoutSessionAsync(
            paymentOrder,
            successUrl,
            cancelUrl,
            cancellationToken);
    }

    public Task<Result<PaymentProcessorPayload, Error>> ProcessAsync(
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
                return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                    new Error(EErrorCode.ValidationError, $"Unsupported Stripe event type: '{stripeEvent.Type}'."));
            }

            if (stripeEvent.Data.Object is not Session session)
            {
                return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                    new Error(EErrorCode.InternalServerError, "Failed to deserialize Stripe session object."));
            }

            if (string.IsNullOrWhiteSpace(session.Id))
            {
                return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                    new Error(EErrorCode.ValidationError, "Missing provider session id in Stripe payload."));
            }

            if (!session.Metadata.TryGetValue("reference_id", out var referenceIdRaw) ||
                !Guid.TryParse(referenceIdRaw, out var referenceId))
            {
                return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                    new Error(EErrorCode.ValidationError, "Missing or invalid 'reference_id' metadata."));
            }

            if (!session.Metadata.TryGetValue("payable_type", out var payableTypeRaw) ||
                !Enum.TryParse<EPayableType>(payableTypeRaw, out var payableType))
            {
                return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                    new Error(EErrorCode.ValidationError, "Missing or invalid 'payable_type' metadata."));
            }

            return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                new PaymentProcessorPayload(referenceId, payableType, session.Id));
        }
        catch (StripeException ex)
        {
            return Task.FromResult<Result<PaymentProcessorPayload, Error>>(
                new Error(EErrorCode.ValidationError, $"Stripe signature verification failed: {ex.Message}"));
        }
    }
}
