using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.DTOs.Product;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbySalto.Junior.Infrastructure.OpenApi;

public class RequestSchemaExamplesFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = context.Type.Name switch
        {
            nameof(CreateProductRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.CreateProduct),
            nameof(UpdateProductRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.UpdateProduct),
            nameof(CreateOrderItemRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.AddOrderItem),
            nameof(UpdateOrderItemRequest) => SwaggerExampleSerializer.FromObject(SwaggerExamplePayloads.UpdateOrderItem),
            _ => schema.Example,
        };
    }
}
