namespace eMechanic.API;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class DependencyInjection
{
    public static void AddApi(this WebApplicationBuilder builder) => builder.AddRedisDistributedCache("emechanic-cache");

    public static void AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        var keyString = configuration["Authentication:JwtBearer:Key"];
        var issuer = configuration["Authentication:JwtBearer:Issuer"];
        var audience = configuration["Authentication:JwtBearer:Audience"];

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!))
                };
            });
    }
}
