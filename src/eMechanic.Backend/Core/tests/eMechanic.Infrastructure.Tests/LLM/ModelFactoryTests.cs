using eMechanic.Infrastructure.LLM.Enums;
using eMechanic.Infrastructure.LLM.Factories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace eMechanic.Infrastructure.Tests.LLM;

public class ModelFactoryTests
{
    private readonly IConfiguration _configuration;
    private readonly ModelFactory _modelFactory;

    public ModelFactoryTests()
    {
        _configuration = Substitute.For<IConfiguration>();
        _modelFactory = new ModelFactory(_configuration);
    }

    [Fact]
    public void GetClient_ShouldReturnOpenAIModel_WhenProviderTypeIsOpenAI()
    {
        // Arrange
        _configuration["LLMProviders:OpenAI:ApiKey"].Returns("fake-api-key");
        _configuration["LLMProviders:OpenAI:Model"].Returns("gpt-4");

        // Act
        var model = _modelFactory.GetClient(ModelProviderType.OpenAI);

        // Assert
        model.Should().NotBeNull();
        model.ProviderType.Should().Be(ModelProviderType.OpenAI);
        model.ModelName.Should().Be("gpt-4");
    }

    [Fact]
    public void GetClient_ShouldReturnGoogleModel_WhenProviderTypeIsGoogle()
    {
        // Arrange
        _configuration["LLMProviders:Google:ApiKey"].Returns("fake-api-key");
        _configuration["LLMProviders:Google:Model"].Returns("gemini-pro");

        // Act
        var model = _modelFactory.GetClient(ModelProviderType.Google);

        // Assert
        model.Should().NotBeNull();
        model.ProviderType.Should().Be(ModelProviderType.Google);
        model.ModelName.Should().Be("gemini-pro");
    }

    [Fact]
    public void GetClient_ShouldThrowInvalidOperationException_WhenApiKeyIsNotConfigured()
    {
        // Arrange
        _configuration["LLMProviders:OpenAI:ApiKey"].Returns(null as string);

        // Act
        var act = () => _modelFactory.GetClient(ModelProviderType.OpenAI);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetClient_ShouldThrowInvalidOperationException_WhenModelNameIsNotConfigured()
    {
        // Arrange
        _configuration["LLMProviders:OpenAI:ApiKey"].Returns("fake-api-key");
        _configuration["LLMProviders:OpenAI:Model"].Returns(null as string);

        // Act
        var act = () => _modelFactory.GetClient(ModelProviderType.OpenAI);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetClient_ShouldThrowNotSupportedException_WhenProviderTypeIsNotSupported()
    {
        _configuration["LLMProviders:None:ApiKey"].Returns("fake-api-key");
        _configuration["LLMProviders:None:Model"].Returns("fake model");

        // Act
        var act = () => _modelFactory.GetClient(ModelProviderType.None);

        // Assert
        act.Should().Throw<NotSupportedException>();
    }
}
