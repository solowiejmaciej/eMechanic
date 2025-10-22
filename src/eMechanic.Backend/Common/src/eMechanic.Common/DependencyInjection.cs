namespace eMechanic.Common;
using Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

public static class ServiceCollectionExtensions
{
    public static void AddAzureAppConfiguration(this IConfigurationBuilder builder)
    {
        if (EnvironmentHelper.IsProduction())
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureAppConfig");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "AzureAppConfig connection string is not set in environment variables.");
            }

            builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(connectionString)
                    .UseFeatureFlags()
                    .Select(KeyFilter.Any);
            });
        }
    }
}
