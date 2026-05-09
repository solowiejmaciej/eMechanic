namespace eMechanic.Infrastructure.LLM.Models;

using Enums;
using LlmTornado;
using LlmTornado.Chat;

public class Model : IModel
{
    public Model(ModelProviderType providerType, string modelName, TornadoApi client)
    {
        ProviderType = providerType;
        ModelName = modelName;
        Client = client;
    }

    public ModelProviderType ProviderType { get; }
    public string ModelName { get; }
    public TornadoApi Client { get; }

    public async Task<string> ExecuteAsync(ChatRequest chatRequest, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await Client.Chat.CreateChatCompletion(chatRequest);
        var choice = result?.Choices?.FirstOrDefault();
        var rawContent = choice?.Message?.Content ?? string.Empty;
        return rawContent;
    }
}

public interface IModel
{
    ModelProviderType ProviderType { get; }
    string ModelName { get; }
    TornadoApi Client { get; }
    Task<string> ExecuteAsync(ChatRequest chatRequest, CancellationToken cancellationToken);
}
