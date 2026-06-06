namespace eMechanic.Application.Payments.Features.Initialize;

using Common;
using eMechanic.Application.Payments.Abstractions;
using eMechanic.Application.Payments.Strategies;
using eMechanic.Common.CQRS;
using eMechanic.Common.Result;

public sealed class InitializePaymentCommandHandler
    : IResultCommandHandler<InitializePaymentCommand, PaymentSessionDto>
{
    private readonly IEnumerable<IPaymentInitializationStrategy> _strategies;
    private readonly IPaymentOrderProcessor _paymentOrderProcessor;

    public InitializePaymentCommandHandler(
        IEnumerable<IPaymentInitializationStrategy> strategies,
        IPaymentOrderProcessor paymentOrderProcessor)
    {
        _strategies = strategies;
        _paymentOrderProcessor = paymentOrderProcessor;
    }

    public async Task<Result<PaymentSessionDto, Error>> Handle(
        InitializePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var strategy = _strategies.FirstOrDefault(s => s.SupportedType == request.Type);

        if (strategy is null)
        {
            return new Error(EErrorCode.ValidationError, $"Unsupported payable type: '{request.Type}'.");
        }

        var payableItemResult = await strategy.BuildPayableItemAsync(request.ReferenceId, cancellationToken);

        if (payableItemResult.HasError())
        {
            return payableItemResult.Error!;
        }

        return await _paymentOrderProcessor.CreateOrGetPendingAsync(
            payableItemResult.Value!,
            request.SuccessUrl,
            request.CancelUrl,
            cancellationToken);
    }
}
