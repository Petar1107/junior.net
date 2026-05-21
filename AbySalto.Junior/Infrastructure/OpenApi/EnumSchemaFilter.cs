using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbySalto.Junior.Infrastructure.OpenApi;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var enumType = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!enumType.IsEnum)
        {
            return;
        }

        schema.Type = "string";
        schema.Format = null;
        schema.Enum.Clear();

        foreach (var name in Enum.GetNames(enumType))
        {
            schema.Enum.Add(new OpenApiString(JsonNamingPolicy.CamelCase.ConvertName(name)));
        }
    }
}
