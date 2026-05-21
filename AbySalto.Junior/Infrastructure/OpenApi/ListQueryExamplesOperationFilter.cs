using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbySalto.Junior.Infrastructure.OpenApi;

public class ListQueryExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor action)
        {
            return;
        }

        if (action.ActionName != "GetAll" || operation.Parameters is null)
        {
            return;
        }

        switch (action.ControllerName)
        {
            case "Products":
                SetParameterExample(operation, "page", new OpenApiInteger(1));
                SetParameterExample(operation, "pageSize", new OpenApiInteger(10));
                SetParameterExample(operation, "search", new OpenApiString("pizza"));
                SetParameterExample(operation, "isActive", new OpenApiBoolean(true));
                SetParameterExample(operation, "sortBy", new OpenApiString("price"));
                SetParameterExample(operation, "sortDirection", new OpenApiString("desc"));
                break;

            case "Orders":
                SetParameterExample(operation, "page", new OpenApiInteger(1));
                SetParameterExample(operation, "pageSize", new OpenApiInteger(20));
                SetParameterExample(operation, "status", new OpenApiString("pending"));
                SetParameterExample(operation, "sortBy", new OpenApiString("totalAmount"));
                SetParameterExample(operation, "sortDirection", new OpenApiString("desc"));
                break;
        }
    }

    private static void SetParameterExample(
        OpenApiOperation operation,
        string name,
        IOpenApiAny example)
    {
        var parameter = operation.Parameters.FirstOrDefault(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

        if (parameter is not null)
        {
            parameter.Example = example;
        }
    }
}
