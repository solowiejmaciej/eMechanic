namespace eMechanic.Application.Payments.Notifications;

using eMechanic.Application.Payments.Strategies;
using MediatR;

public sealed class PaymentConfirmedNotificationHandler : INotificationHandler<PaymentConfirmedNotification>
{
    private readonly IEnumerable<IPaymentConfirmationStrategy> _strategies;

    public PaymentConfirmedNotificationHandler(IEnumerable<IPaymentConfirmationStrategy> strategies)
    {
        _strategies = strategies;
    }

    public async Task Handle(PaymentConfirmedNotification notification, CancellationToken cancellationToken)
    {
        var strategy = _strategies.FirstOrDefault(s => s.SupportedType == notification.Type);

        if (strategy is null)
        {
            return;
        }

        await strategy.HandleAsync(notification.ReferenceId, cancellationToken);
    }
}

