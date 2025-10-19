namespace eMechanic.API.Features;

public static class Testing
{
    public static void MapTesting(this WebApplication app)
    {
        app.MapGet("/ping", () => Results.Ok("pong"))
            .WithName("Ping")
            .WithTags("Ping");
    }
}
