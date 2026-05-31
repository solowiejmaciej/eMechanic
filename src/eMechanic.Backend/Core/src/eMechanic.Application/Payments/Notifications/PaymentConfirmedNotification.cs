namespace eMechanic.Application.Payments.Notifications;

using Common;
using MediatR;

public sealed record PaymentConfirmedNotification(Guid ReferenceId, EPayableType Type) : INotification;

