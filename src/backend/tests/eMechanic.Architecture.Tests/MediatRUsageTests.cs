namespace eMechanic.Architecture.Tests;

using NetArchTest.Rules;
using MediatR;
using Common.Result;
using System.Reflection;
using Common.CQRS;

public class MediatRUsageTests
{
    private const string MEDIATR_NAMESPACE = "MediatR";
    private const string APPLICATION_NAMESPACE = "eMechanic.Application";

    private static readonly Assembly[] SolutionAssemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.FullName != null && a.FullName.StartsWith("eMechanic.", StringComparison.Ordinal))
        .ToArray();

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
        // POPRAWKA: Skanujemy wszystkie assembly, a nie tylko 'Common'
        var result = Types.InAssemblies(SolutionAssemblies)
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

    [Fact]
    public void Handlers_ShouldResideInApplicationProject()
    {
        // Act
        var result = Types.InAssemblies(SolutionAssemblies)
            .That()
            .ImplementInterface(typeof(IResultQueryHandler<,>))
            .Or()
            .ImplementInterface(typeof(IResultCommandHandler<,>))
            .Should()
            .ResideInNamespaceStartingWith(APPLICATION_NAMESPACE)
            .GetResult();

        // Assert
        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes.Select(t => t.FullName);
            var errorMessage = $"All Handlers (IResultQueryHandler/IResultCommandHandler) must reside within the '{APPLICATION_NAMESPACE}' namespace. Failing types found:\n{string.Join("\n", failingTypes)}";
            Assert.Fail(errorMessage);
        }
    }
}
