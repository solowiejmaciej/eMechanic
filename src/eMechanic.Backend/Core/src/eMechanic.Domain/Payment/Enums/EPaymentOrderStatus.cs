namespace eMechanic.Domain.Payment.Enums;

public enum EPaymentOrderStatus
{
    Created,
    CheckoutStarted,
    Paid,
    Failed,
    Cancelled,
    Expired,
}
