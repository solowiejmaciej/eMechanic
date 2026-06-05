namespace eMechanic.Application.Payments.Common;

using Domain.Payment.Enums;

public sealed record PaymentOrderCompletionResult(Guid ReferenceId, EPayableType Type, bool IsNewlyCompleted);
