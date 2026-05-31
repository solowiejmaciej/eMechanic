namespace eMechanic.API.Features.Payment.Webhook;

using Application.Payments.Features.ProcessWebhook;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public sealed class PaymentWebhookFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(PaymentPrefix.WEBHOOK, async (
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                using var reader = new System.IO.StreamReader(
                    httpContext.Request.Body,
                    System.Text.Encoding.UTF8);

                var jsonPayload = await reader.ReadToEndAsync(cancellationToken);
                var signatureHeader = httpContext.Request.Headers["Stripe-Signature"]
                    .FirstOrDefault() ?? string.Empty;

                var command = new ProcessPaymentWebhookCommand(jsonPayload, signatureHeader);
                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(_ => Results.Ok(), MapError);
            })
            .WithName("PaymentWebhook")
            .WithTags(PaymentPrefix.TAG)
            .Produces(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Receives and processes Stripe payment webhook events.")
            .AllowAnonymous();
    }
}


