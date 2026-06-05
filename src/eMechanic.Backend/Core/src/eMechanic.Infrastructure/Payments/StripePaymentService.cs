namespace eMechanic.Infrastructure.Payments;

using eMechanic.Application.Payments.Common;
using eMechanic.Common.Result;
using eMechanic.Domain.Payment;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

internal sealed class StripePaymentService : IStripePaymentService
{
    private readonly IOptions<StripeOptions> _options;

    public StripePaymentService(IOptions<StripeOptions> options)
    {
        _options = options;
    }

    public async Task<Result<PaymentSessionDto, Error>> CreateCheckoutSessionAsync(
        PaymentOrder paymentOrder,
        string successUrl,
        string cancelUrl,
        CancellationToken cancellationToken)
    {
        if (!IsAbsoluteHttpUrl(successUrl))
        {
            return new Error(EErrorCode.ValidationError, "SuccessUrl must be an absolute HTTP/HTTPS URL.");
        }

        if (!IsAbsoluteHttpUrl(cancelUrl))
        {
            return new Error(EErrorCode.ValidationError, "CancelUrl must be an absolute HTTP/HTTPS URL.");
        }

        try
        {
            var sessionOptions = new SessionCreateOptions
            {
                Mode = "payment",
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(paymentOrder.Amount.Amount * 100),
                            Currency = paymentOrder.Amount.Currency.ToLowerInvariant(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{paymentOrder.PayableType} Payment",
                            },
                        },
                        Quantity = 1,
                    },
                ],
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    ["reference_id"] = paymentOrder.ReferenceId.ToString(),
                    ["payable_type"] = paymentOrder.PayableType.ToString(),
                    ["payer_id"] = paymentOrder.PayerId.ToString(),
                },
            };

            var requestOptions = new RequestOptions { ApiKey = _options.Value.SecretKey };
            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions, requestOptions, cancellationToken: cancellationToken);

            return new PaymentSessionDto(session.Id, session.Url);
        }
        catch (StripeException ex)
        {
            return new Error(EErrorCode.InternalServerError, $"Stripe error: {ex.Message}");
        }
    }

    private static bool IsAbsoluteHttpUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}
