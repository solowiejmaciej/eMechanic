
using System;
using System.Threading;
using System.Threading.Tasks;
using eMechanic.Infrastructure.LLM.Services;
using eMechanic.Infrastructure.LLM.Models;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace eMechanic.Infrastructure.Tests.LLM;

public class ModelFacadeTests
{
    private readonly IModel _model;
    private readonly ModelFacade _sut;

    public ModelFacadeTests()
    {
        _model = Substitute.For<IModel>();
        _sut = new ModelFacade(_model);
    }

    [Fact(Skip = "Not used right now")]
    public async Task GetResponseAsync_ShouldReturnDevelopmentMessage_WhenInDevelopment()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        // Act
        var result = await _sut.GetResponseAsync("system", "user", CancellationToken.None);

        // Assert
        result.Should().Be("AI report generation is disabled in development environment.");
        await _model.DidNotReceive().ExecuteAsync(Arg.Any<LlmTornado.Chat.ChatRequest>(), Arg.Any<CancellationToken>());

        // Cleanup
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
    }

    [Fact]
    public async Task GetResponseAsync_ShouldCallExecuteAsync_WhenNotInDevelopment()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
        var expectedResponse = "AI response";
        _model.ExecuteAsync(Arg.Any<LlmTornado.Chat.ChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResponse));
        _model.ModelName.Returns("test-model");

        // Act
        var result = await _sut.GetResponseAsync("system", "user", CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        await _model.Received(1).ExecuteAsync(Arg.Any<LlmTornado.Chat.ChatRequest>(), Arg.Any<CancellationToken>());

        // Cleanup
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
    }
}
