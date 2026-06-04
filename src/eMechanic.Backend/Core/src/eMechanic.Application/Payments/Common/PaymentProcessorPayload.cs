namespace eMechanic.Application.Payments.Common;

using Domain.Payment.Enums;

public sealed record PaymentProcessorPayload(Guid ReferenceId, EPayableType Type, string ProviderSessionId);
