namespace eMechanic.API.Swagger;

using System;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        var enumNames = Enum.GetNames(context.Type);
        if (enumNames.Length == 0)
        {
            return;
        }

        var descriptionBuilder = new StringBuilder();

        if (!string.IsNullOrEmpty(schema.Description))
        {
            descriptionBuilder.AppendLine(schema.Description);
        }

        descriptionBuilder.AppendLine("<br><b>Value:</b><ul>");

        foreach (var name in enumNames)
        {
            var value = Convert.ToInt64(Enum.Parse(context.Type, name), System.Globalization.CultureInfo.InvariantCulture);
            descriptionBuilder.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"<li><b>{value}</b> = {name}</li>");
        }

        descriptionBuilder.AppendLine("</ul>");
        schema.Description = descriptionBuilder.ToString();
    }
}

