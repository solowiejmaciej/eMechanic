namespace eMechanic.API.Features;
using System.Reflection;

public static class FeatureExtensions
{
    public static IEndpointRouteBuilder MapFeatures(this IEndpointRouteBuilder app)
    {
        var featureTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true } && typeof(IFeature).IsAssignableFrom(t));

        foreach (var featureType in featureTypes)
        {
            if (Activator.CreateInstance(featureType) is IFeature feature)
            {
                feature.MapEndpoint(app);
            }
        }

        return app;
    }
}
