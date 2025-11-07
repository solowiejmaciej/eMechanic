namespace eMechanic.Architecture.Tests;
using API.Features;
using Common.Web;
using NetArchTest.Rules;
using Xunit;

public class FeatureArchitectureTests
{
    private const string FEATURES_NAMESPACE = "eMechanic.API.Features";

    [Fact]
    public void Features_ShouldBeInFeaturesNamespace()
    {
        var result = Types.InAssembly(typeof(IFeature).Assembly)
            .That()
            .ImplementInterface(typeof(IFeature))
            .Should()
            .ResideInNamespace(FEATURES_NAMESPACE)
            .Or().ResideInNamespaceStartingWith($"{FEATURES_NAMESPACE}.")
            .GetResult();

        Assert.True(result.IsSuccessful, "All classes implementing IFeature must reside within the Features namespace.");
    }

    [Fact]
    public void FeatureClasses_ShouldBeSealed()
    {
        var result = Types.InAssembly(typeof(IFeature).Assembly)
            .That()
            .ImplementInterface(typeof(IFeature))
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, "All feature classes should be marked as sealed to maintain architecture integrity.");
    }

    [Fact]
    public void FeatureClasses_ShouldNotDependOnExternalLogic()
    {
        var result = Types.InAssembly(typeof(IFeature).Assembly)
            .That()
            .ImplementInterface(typeof(IFeature))
            .ShouldNot()
            .HaveDependencyOn("eMechanic.Infrastructure")
            .And()
            .HaveDependencyOn("eMechanic.Core.Domain")
            .GetResult();

        Assert.True(result.IsSuccessful, "Feature classes should only contain routing/mapping logic and rely on MediatR, not direct domain or infrastructure access.");
    }
}
