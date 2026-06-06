namespace eMechanic.NotificationService.Features.Test.Get;

using Common.CQRS;
using Common.Result;


public class SendTestNotificationComand : IResultCommand<Success>;

public class SendTestNotificationComandHandler: IResultCommandHandler<SendTestNotificationComand, Success>
{
    public async Task<Result<Success, Error>> Handle(SendTestNotificationComand request, CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        Console.WriteLine("ok");
        return Result.Success;
    }
}
