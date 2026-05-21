using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Any;

namespace AbySalto.Junior.Infrastructure.OpenApi;

internal static class SwaggerExampleSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public static IOpenApiAny FromObject(object value)
    {
        var json = JsonSerializer.Serialize(value, Options);
        using var document = JsonDocument.Parse(json);
        return ToOpenApiAny(document.RootElement);
    }

    private static IOpenApiAny ToOpenApiAny(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.Object => ToOpenApiObject(element),
            JsonValueKind.Array => ToOpenApiArray(element),
            JsonValueKind.String => new OpenApiString(element.GetString()),
            JsonValueKind.Number when element.TryGetInt64(out var integer) => new OpenApiLong(integer),
            JsonValueKind.Number => new OpenApiDouble(element.GetDouble()),
            JsonValueKind.True => new OpenApiBoolean(true),
            JsonValueKind.False => new OpenApiBoolean(false),
            _ => new OpenApiNull(),
        };

    private static OpenApiObject ToOpenApiObject(JsonElement element)
    {
        var result = new OpenApiObject();
        foreach (var property in element.EnumerateObject())
        {
            result[property.Name] = ToOpenApiAny(property.Value);
        }

        return result;
    }

    private static OpenApiArray ToOpenApiArray(JsonElement element)
    {
        var result = new OpenApiArray();
        foreach (var item in element.EnumerateArray())
        {
            result.Add(ToOpenApiAny(item));
        }

        return result;
    }
}
