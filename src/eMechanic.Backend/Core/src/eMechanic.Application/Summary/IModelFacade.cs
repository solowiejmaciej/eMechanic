namespace eMechanic.Application.Summary;

public interface IModelFacade
{
    Task<string> GetResponseAsync(string systemPrompt, string dataPrompt, CancellationToken cancellationToken);
}
