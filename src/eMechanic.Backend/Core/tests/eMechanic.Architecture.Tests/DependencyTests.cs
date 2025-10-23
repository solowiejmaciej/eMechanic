namespace eMechanic.Architecture.Tests;

using NetArchTest.Rules;

public class DependencyTests : ArchitectureTestBase
{
    [Fact]
    public void Application_Should_Not_DependOnApi()
    {
        var applicationAssembly = GetAssembly(APPLICATION_NAMESPACE);

        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(API_NAMESPACE)
            .GetResult();

        Assert.True(result.IsSuccessful, "Application layer should not depend on API layer.");
    }

    [Fact]
    public void Common_Should_Not_DependOnOtherLayers()
    {
        var commonAssembly = GetAssembly(COMMON_NAMESPACE);

        var result = Types.InAssembly(commonAssembly)
            .ShouldNot()
            .HaveDependencyOn(API_NAMESPACE)
            .And()
            .HaveDependencyOn(APPLICATION_NAMESPACE)
            .And()
            .HaveDependencyOn(DOMAIN_NAMESPACE)
            .And()
            .HaveDependencyOn(INFRASTRUCTURE_NAMESPACE)
            .GetResult();

        Assert.True(result.IsSuccessful, "Common layer should not depend on other layers.");
    }

    [Fact]
    public void Domain_Should_Not_DependOnOtherLayers()
    {
        var domainAssembly = GetAssembly(DOMAIN_NAMESPACE);

        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn(API_NAMESPACE)
            .And()
            .HaveDependencyOn(APPLICATION_NAMESPACE)
            .And()
            .HaveDependencyOn(INFRASTRUCTURE_NAMESPACE)
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain layer should not depend on other layers.");
    }

    [Fact]
    public void Features_Should_Not_DependOnInfrastructure()
    {
        var apiAssembly = GetAssembly(API_NAMESPACE);

        var result = Types.InAssembly(apiAssembly)
            .That()
            .ResideInNamespaceContaining(FEATURES_NAMESPACE)
            .ShouldNot()
            .HaveDependencyOn(INFRASTRUCTURE_NAMESPACE)
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "API Features should not depend on Infrastructure layer. All infrastructure interactions should be handled in the Application layer."
        );
    }
}
