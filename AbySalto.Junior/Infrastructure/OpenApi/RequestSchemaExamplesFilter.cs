using AbySalto.Junior.Application.DTOs.Auth;
using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.DTOs.Product;
using AbySalto.Junior.Infrastructure.Database.Seed;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbySalto.Junior.Infrastructure.OpenApi;

public class RequestSchemaExamplesFilter : ISchemaFilter
{
    private readonly IConfiguration _configuration;

    public RequestSchemaExamplesFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = context.Type.Name switch
        {
            nameof(LoginRequest) => SwaggerExampleSerializer.FromObject(
                SwaggerExamplePayloads.SeededAdminLogin(_configuration.GetSection("Seed").Get<SeedSettings>())),
            nameof(RegisterRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.Register),
            nameof(CreateProductRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.CreateProduct),
            nameof(UpdateProductRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.UpdateProduct),
            nameof(CreateOrderItemRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.AddOrderItem),
            nameof(UpdateOrderItemRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.UpdateOrderItem),
            _ => schema.Example,
        };
    }
}
