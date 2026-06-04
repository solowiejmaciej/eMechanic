namespace eMechanic.Application.Payments.Notifications;

using Domain.Payment.Enums;
using MediatR;

public sealed record PaymentConfirmedNotification(Guid ReferenceId, EPayableType Type) : INotification;
