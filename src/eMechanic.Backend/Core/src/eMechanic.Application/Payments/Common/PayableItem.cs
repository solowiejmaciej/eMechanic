namespace eMechanic.Application.Payments.Common;

using eMechanic.Domain.Shared.ValueObjects;

public sealed record PayableItem(Guid ReferenceId, EPayableType Type, Money Amount, Guid PayerId);

