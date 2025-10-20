namespace eMechanic.Architecture.Tests;
using NetArchTest.Rules;
using MediatR;
using Common.Result;

public class MediatRUsageTests
{
    private const string MEDIATR_NAMESPACE = "MediatR";

    // Lista nazw interfejsów bazowych, które MUSZĄ dziedziczyć z MediatR, więc mają referencję.
    private static readonly string[] CustomMediatRBaseInterfaces = new[]
    {
        "IResultCommand",
        "IResultQuery",
        "IResultCommandHandler",
        "IResultQueryHandler"
    };

    [Fact]
    public void AllHandlers_ShouldImplementCustomInterfacesOnly()
    {
        var assembly = typeof(Error).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .DoNotHaveName(CustomMediatRBaseInterfaces)
            .ShouldNot()
            .ImplementInterface(typeof(IRequest))
            .And().ImplementInterface(typeof(IRequestHandler<,>))
            .And().ImplementInterface(typeof(IRequestHandler<>))
            .And().ImplementInterface(typeof(IStreamRequest<>))
            .GetResult();

        Assert.True(result.IsSuccessful, "All CQRS Requests and Handlers must use custom eMechanic interfaces (IResultQuery/Command/Handler) and MUST NOT implement raw MediatR interfaces directly.");
    }

    [Fact]
    public void ProjectCommon_ShouldNotReferenceMediatRInterfacesDirectly()
    {
        var assembly = typeof(Error).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .DoNotHaveName(CustomMediatRBaseInterfaces)
            .And()
            .AreClasses()
            .ShouldNot()
            .HaveDependencyOn(MEDIATR_NAMESPACE)
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes.Select(t => t.FullName);
            var errorMessage = $"Direct usage of MediatR types is forbidden in the Common project outside of custom base interfaces (IResult...). Use the wrapper interfaces instead. Failing types found:\n{string.Join("\n", failingTypes)}";
            Assert.Fail(errorMessage);
        }
    }
}
