namespace eMechanic.Application.Payments.Features.Process;

using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Notifications;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;
using MediatR;

public sealed class ProcessPaymentCommandHandler
    : IResultCommandHandler<ProcessPaymentCommand, Success>
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IPaymentOrderProcessor _paymentOrderProcessor;
    private readonly IPublisher _publisher;

    public ProcessPaymentCommandHandler(
        IPaymentProcessor paymentProcessor,
        IPaymentOrderProcessor paymentOrderProcessor,
        IPublisher publisher)
    {
        _paymentProcessor = paymentProcessor;
        _paymentOrderProcessor = paymentOrderProcessor;
        _publisher = publisher;
    }

    public async Task<Result<Success, Error>> Handle(
        ProcessPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var processorResult = await _paymentProcessor.ProcessAsync(
            request.JsonPayload,
            request.SignatureHeader,
            cancellationToken);

        if (processorResult.HasError())
        {
            return processorResult.Error!;
        }

        var payload = processorResult.Value!;

        var completionResult = await _paymentOrderProcessor.CompleteAsync(payload, cancellationToken);

        if (completionResult.HasError())
        {
            return completionResult.Error!;
        }

        var completion = completionResult.Value!;

        if (!completion.IsNewlyCompleted)
        {
            return Result.Success;
        }

        await _publisher.Publish(
            new PaymentConfirmedNotification(completion.ReferenceId, completion.Type),
            cancellationToken);

        return Result.Success;
    }
}
