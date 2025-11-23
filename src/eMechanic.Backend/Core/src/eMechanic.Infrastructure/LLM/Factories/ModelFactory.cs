namespace eMechanic.Infrastructure.LLM.Factories;

using Enums;
using LlmTornado;
using LlmTornado.Code;
using Microsoft.Extensions.Configuration;
using Models;

public class ModelFactory : IModelFactory
{
    private readonly IConfiguration _configuration;

    public ModelFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IModel GetClient(ModelProviderType providerType)
    {
        var apiKey = _configuration["LLMProviders:" +providerType + ":ApiKey"];
        var modelName = _configuration["LLMProviders:" +providerType + ":Model"];

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException($"API key for provider type '{providerType}' is not configured.");
        }

        if (modelName != null)
        {
            return providerType switch
            {
                ModelProviderType.OpenAI => new Model(ModelProviderType.OpenAI, modelName, new TornadoApi(apiKey)),
                ModelProviderType.Google => new Model(ModelProviderType.Google, modelName,
                    new TornadoApi(apiKey, LLmProviders.Google)),
                _ => throw new NotSupportedException($"Model provider type '{providerType}' is not supported.")
            };
        }
        throw new InvalidOperationException($"Model name for provider type '{providerType}' is not configured.");
    }
}

public interface IModelFactory
{
    IModel GetClient(ModelProviderType providerType);
}
