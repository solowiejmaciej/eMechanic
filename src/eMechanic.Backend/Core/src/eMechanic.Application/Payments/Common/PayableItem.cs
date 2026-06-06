namespace eMechanic.Application.Payments.Common;

using Domain.Payment.Enums;
using eMechanic.Domain.Shared.ValueObjects;

public sealed record PayableItem(Guid ReferenceId, EPayableType Type, Money Amount, Guid PayerId);
