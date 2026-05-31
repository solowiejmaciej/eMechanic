namespace eMechanic.Application.Payments.Features.ProcessWebhook;

using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Notifications;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using MediatR;

public sealed class ProcessPaymentWebhookCommandHandler
    : IResultCommandHandler<ProcessPaymentWebhookCommand, Success>
{
    private readonly IPaymentWebhookProcessor _webhookProcessor;
    private readonly IPublisher _publisher;

    public ProcessPaymentWebhookCommandHandler(
        IPaymentWebhookProcessor webhookProcessor,
        IPublisher publisher)
    {
        _webhookProcessor = webhookProcessor;
        _publisher = publisher;
    }

    public async Task<Result<Success, Error>> Handle(
        ProcessPaymentWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var processorResult = await _webhookProcessor.ProcessAsync(
            request.JsonPayload,
            request.SignatureHeader,
            cancellationToken);

        if (processorResult.HasError())
        {
            return processorResult.Error!;
        }

        var payableItem = processorResult.Value!;

        var notification = new PaymentConfirmedNotification(payableItem.ReferenceId, payableItem.Type);
        await _publisher.Publish(notification, cancellationToken);

        return Result.Success;
    }
}

