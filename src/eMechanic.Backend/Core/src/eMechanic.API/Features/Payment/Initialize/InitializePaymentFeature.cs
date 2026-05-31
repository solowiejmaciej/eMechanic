namespace eMechanic.API.Features.Payment.Initialize;

using Application.Payments.Common;
using Application.Payments.Features.Initialize;
using Common.Result;
using Common.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Security;

public sealed class InitializePaymentFeature : IFeature
{
    public IResult MapError(Error error) => ErrorMapper.MapToHttpResult(error);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(PaymentPrefix.INITIALIZE, async (
                [FromBody] InitializePaymentRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new InitializePaymentCommand(
                    request.ReferenceId,
                    request.Type,
                    request.SuccessUrl,
                    request.CancelUrl);

                var result = await mediator.Send(command, cancellationToken);
                return result.ToStatusCode(dto => Results.Ok(dto), MapError);
            })
            .WithName("InitializePayment")
            .WithTags(PaymentPrefix.TAG)
            .Produces<PaymentSessionDto>()
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Creates a Stripe checkout session for a payable item.")
            .RequireAuthorization(AuthorizationPolicies.MUST_BE_USER);
    }
}

public sealed record InitializePaymentRequest(
    Guid ReferenceId,
    EPayableType Type,
    string SuccessUrl,
    string CancelUrl);

