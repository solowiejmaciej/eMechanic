
using eMechanic.Infrastructure.LLM.Builders;
using FluentAssertions;
using LlmTornado.Chat;
using LlmTornado.Code;
using Xunit;

namespace eMechanic.Infrastructure.Tests.LLM;

public class ChatRequestBuilderTests
{
    [Fact]
    public void Build_ShouldCreateChatRequest_WithCorrectProperties()
    {
        // Arrange
        var builder = new ChatRequestBuilder();
        const string modelName = "test-model";
        const string userMessage = "Hello, world!";
        const string systemMessage = "You are a helpful assistant.";

        // Act
        var chatRequest = builder
            .WithModel(modelName)
            .WithUserMessage(userMessage)
            .WithSystemMessage(systemMessage)
            .Build();

        // Assert
        chatRequest.Model?.Name.Should().Be(modelName);
        chatRequest.Messages.Should().HaveCount(2);
        chatRequest.Messages[0].Role.Should().Be(ChatMessageRoles.User);
        chatRequest.Messages[0].Content.Should().Be(userMessage);
        chatRequest.Messages[1].Role.Should().Be(ChatMessageRoles.System);
        chatRequest.Messages[1].Content.Should().Be(systemMessage);
    }

    [Fact]
    public void Constructor_WithPreviousMessages_ShouldInitializeMessages()
    {
        // Arrange
        var previousMessages = new List<ChatMessage>
        {
            new(ChatMessageRoles.User, "Previous message")
        };

        // Act
        var builder = new ChatRequestBuilder(previousMessages);
        var chatRequest = builder.Build();

        // Assert
        chatRequest.Messages.Should().HaveCount(1);
        chatRequest.Messages[0].Role.Should().Be(ChatMessageRoles.User);
        chatRequest.Messages[0].Content.Should().Be("Previous message");
    }
}
