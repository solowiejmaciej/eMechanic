namespace eMechanic.API;

using System.Text;
using Application.Identity;
using Common.Result;
using Common.Web;
using Constans;
using Infrastructure;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middleware;
using Security;
using ServiceDefaults;
using Swagger;

public static class DependencyInjection
{
    private const string GIT_HUB_ICON_BASE64 = "data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxNiIgaGVpZ2h0PSIxNiIgZmlsbD0iY3VycmVudENvbG9yIiB2aWV3Qm94PSIwIDAgMTYgMTYiPjxwYXRoIGQ9Ik04IDBDMy41OCAwIDAgMy41OCAwIDhjMCAzLjU0IDIuMjkgNi41MyA1LjQ3IDcuNTkuNC4wNy41NS0uMTcuNTUtLjM4IDAtLjE5LS4wMS0uODItLjAxLTEuNDktMi4wMS4zNy0yLjUzLS40OS0yLjY5LS45NC0uMDktLjIzLS40OC0uOTQtLjgyLTEuMTMtLjI4LS4xNS0uNjgtLjUyLS4wMS0uNTMuNjMtLjAxIDEuMDguNTggMS4yMy44Mi43MiAxLjIxIDEuODcuODcgMi4zMy42Ni4wNy0uNTIuMjgtLjg3LjUxLTEuMDctMS43OC0uMi0zLjY0LS44OS0zLjY0LTMuOTUgMC0uODcuMzEtMS41OS44Mi0yLjE1LS4wOC0uMi0uMzYtMS4wMi4wOC0yLjEyIDAgMCAuNjctLjIxIDIuMi44Mi42NC0uMTggMS4zMi0uMjcgMi0uMjcuNjggMCAxLjM2LjA5IDIgLjI3IDEuNTMtMS4wNCAyLjItLjgyIDIuMi0uODIuNDQgMS4xLjE2IDEuOTIuMDggMi4xMi41MS41Ni44MiAxLjI3LjgyIDIuMTUgMCAzLjA3LTEuODcgMy43NS0zLjY1IDMuOTUuMjkuMjUuNTQuNzMuNTQgMS40OCAwIDEuMDctLjAxIDEuOTMtLjAxIDIuMiAwIC4yMS4xNS40Ni41NS4zOEExOC4wMTIgOC4wMTIgMCAwIDAgMTYgOGMwLTQuNDItMy41OC04LTgtOHoiLz48L3N2Zz4=";
    public static void AddApi(this WebApplicationBuilder builder) => builder.AddRedisDistributedCache("emechanic-cache");

    public static void AddSwagger(this IServiceCollection services, string title, string version)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = GenerateDescription()
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });

            c.SchemaFilter<EnumSchemaFilter>();
        });
    }

    private static string GenerateDescription()
    {
        var descriptionBuilder = new StringBuilder();

        string logoUrl = "https://emechanic01.blob.core.windows.net/assets/images/emechanic-logo.png";

        descriptionBuilder.AppendLine(
            System.Globalization.CultureInfo.InvariantCulture,
            $"<img src='{logoUrl}' alt='eMechanic Logo' width='200' style='display: block; margin: 0 auto; margin-bottom: 20px;' />"
        );

        descriptionBuilder.AppendLine("<br>");
        descriptionBuilder.AppendLine("Standard error codes returned by the API");
        descriptionBuilder.AppendLine("<br><br><b>Error Codes:</b><ul>");

        var errorCodes = Enum.GetValues<EErrorCode>();

        foreach (var code in errorCodes)
        {
            if (code == EErrorCode.None)
            {
                continue;
            }

            string name = code.ToString();
            int value = (int)code;

            descriptionBuilder.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"<li><b>{name} ({value})</b></li>");
        }

        descriptionBuilder.AppendLine("</ul>");

        descriptionBuilder.AppendLine("<hr>");
        descriptionBuilder.AppendLine("<small><i>eMechanic API developed by Maciej Sołowiej.</i></small>");

        descriptionBuilder.AppendLine("<br>");
        descriptionBuilder.AppendLine(
            $"<small><i><img src='{GIT_HUB_ICON_BASE64}' alt='GitHub Icon' width='14' height='14' style='vertical-align:middle; margin-right: 4px;'> <a href='https://github.com/solowiejmaciej/eMechanic' target='_blank'>GitHub Repository</a></i></small>"
        );
        return descriptionBuilder.ToString();
    }
    public static void AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddSwagger("eMechanic API", WebApiConstans.CURRENT_API_VERSION);

        var keyString = configuration["Authentication:JwtBearer:Key"];
        var issuer = configuration["Authentication:JwtBearer:Issuer"];
        var audience = configuration["Authentication:JwtBearer:Audience"];

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.MUST_BE_USER, policy =>
                policy.RequireClaim(ClaimConstants.IDENTITY_TYPE, nameof(EIdentityType.User)));

            options.AddPolicy(AuthorizationPolicies.MUST_BE_WORKSHOP, policy =>
                policy.RequireClaim(ClaimConstants.IDENTITY_TYPE, nameof(EIdentityType.Workshop)));
        });

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

    public static void AddApi(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.MapDefaultEndpoints();
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.InjectStylesheet("/swagger-custom.css");
            c.DefaultModelsExpandDepth(0);
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });

        app.UseAuthentication();
        app.UseAuthorization();

        var apiV1Group = app.MapGroup($"/api/{WebApiConstans.CURRENT_API_VERSION}");
        apiV1Group.MapFeatures();

        app.Services.ApplyMigrations();
    }
}
