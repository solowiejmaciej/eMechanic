namespace eMechanic.Infrastructure.LLM.Services;

using Application.Summary;
using Builders;
using Common.Helpers;
using Models;

public class ModelFacade : IModelFacade
{
    private readonly IModel _model;

    public ModelFacade(IModel model)
    {
        _model = model;
    }

    public Task<string> GetResponseAsync(string systemPrompt, string dataPrompt, CancellationToken cancellationToken)
    {
        if (EnvironmentHelper.IsDevelopment())
        {
            return Task.FromResult("AI report generation is disabled in development environment.");
        }

        var chatRequest = new ChatRequestBuilder()
            .WithModel(_model.ModelName)
            .WithSystemMessage(systemPrompt)
            .WithUserMessage(dataPrompt)
            .Build();

        return _model.ExecuteAsync(chatRequest, cancellationToken);
    }
}
