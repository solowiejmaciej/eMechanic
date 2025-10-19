namespace eMechanic.API.Features;

public static class FeatureExtensions
{
    public static WebApplication MapFeatures(this WebApplication app)
    {
        app.MapTesting();
        return app;
    }
}
