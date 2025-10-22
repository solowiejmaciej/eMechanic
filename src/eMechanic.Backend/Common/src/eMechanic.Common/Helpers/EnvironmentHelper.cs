namespace eMechanic.Common.Helpers;

public static class EnvironmentHelper
{
    public static bool IsProduction() => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

    public static bool IsDevelopment() => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
}
