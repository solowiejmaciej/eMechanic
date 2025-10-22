namespace eMechanic.Architecture.Tests;

using Common.CQRS;
using NetArchTest.Rules;

public class NamingConventionTests : ArchitectureTestBase
{
    [Fact]
    public void Queries_Should_BeSealedAndEndWithQuery()
    {
        var result = Types.InAssemblies(SolutionAssemblies)
            .That()
            .ImplementInterface(typeof(IResultQuery<>))
            .Should()
            .BeSealed()
            .And()
            .HaveNameEndingWith("Query")
            .GetResult();

        Assert.True(result.IsSuccessful, "All classes implementing IResultQuery should be 'sealed' and have suffix 'Query'.");
    }

    [Fact]
    public void Commands_Should_BeSealedAndEndWithCommand()
    {
        var result = Types.InAssemblies(SolutionAssemblies)
            .That()
            .ImplementInterface(typeof(IResultCommand<>))
            .Should()
            .BeSealed()
            .And()
            .HaveNameEndingWith("Command")
            .GetResult();

        Assert.True(result.IsSuccessful, "All classes implementing IResultCommand should be 'sealed' and have suffix 'Command'.");
    }

    [Fact]
    public void Handlers_Should_EndWithHandler()
    {
        var result = Types.InAssemblies(SolutionAssemblies)
            .That()
            .ImplementInterface(typeof(IResultQueryHandler<,>))
            .Or()
            .ImplementInterface(typeof(IResultCommandHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        Assert.True(result.IsSuccessful, "All classes implementing IResultQueryHandler or IResultCommandHandler should have suffix 'Handler'.");
    }
}
