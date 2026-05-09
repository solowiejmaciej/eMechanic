namespace eMechanic.Infrastructure.LLM.Builders;

using LlmTornado.Chat;
using LlmTornado.Code;

public class ChatRequestBuilder
{
    private readonly ChatRequest _chatRequest;

    public ChatRequestBuilder()
    {
        _chatRequest = new ChatRequest
        {
            Messages = new List<ChatMessage>()
        };
    }

    public ChatRequestBuilder(List<ChatMessage> previousMessages)
    {
        _chatRequest = new ChatRequest
        {
            Messages = new List<ChatMessage>(previousMessages)
        };
    }

    public ChatRequestBuilder WithModel(string modelName)
    {
        _chatRequest.Model = modelName;
        return this;
    }

    public ChatRequestBuilder WithUserMessage(string content)
    {
        _chatRequest.Messages?.Add(new ChatMessage(ChatMessageRoles.User, content));
        return this;
    }

    public ChatRequestBuilder WithSystemMessage(string content)
    {
        _chatRequest.Messages?.Add(new ChatMessage(ChatMessageRoles.System, content));
        return this;
    }

    public ChatRequest Build() => _chatRequest;
}
