using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AbySalto.Junior.Infrastructure.OpenApi;

public class RequestBodyExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor action)
        {
            return;
        }

        switch (action.ControllerName, action.ActionName)
        {
            case ("Orders", "Create"):
                SetJsonExamples(
                    operation,
                    new Dictionary<string, (string Summary, string Description, object Body)>
                    {
                        ["guestOrder"] = (
                            "Guest / walk-in",
                            "CustomerName without CustomerId (typical Customer flow).",
                            SwaggerExamplePayloads.GuestCreateOrder),
                        ["registeredCustomer"] = (
                            "Registered customer",
                            "CustomerId links order to account (Admin or matching Customer).",
                            SwaggerExamplePayloads.RegisteredCustomerCreateOrder),
                    });
                AppendDescription(operation, SwaggerExamplePayloads.PlaceholderGuidNote);
                break;

            case ("Orders", "Update"):
                SetJsonExamples(
                    operation,
                    new Dictionary<string, (string Summary, string Description, object Body)>
                    {
                        ["headerFields"] = (
                            "Header fields (Pending only)",
                            "CustomerName, payment, contact — while order status is pending.",
                            SwaggerExamplePayloads.UpdateOrderHeader),
                        ["statusTransition"] = (
                            "Status transition (Admin)",
                            "Forward-only: pending → inPreparation → completed.",
                            SwaggerExamplePayloads.UpdateOrderStatus),
                    });
                break;

            case ("Orders", "AddItem"):
                SetSingleJsonExample(operation, SwaggerExamplePayloads.AddOrderItem);
                AppendDescription(operation, SwaggerExamplePayloads.PlaceholderGuidNote);
                break;
        }
    }

    private static void SetSingleJsonExample(OpenApiOperation operation, object body)
    {
        if (!TryGetJsonMediaType(operation, out var mediaType))
        {
            return;
        }

        mediaType.Example = SwaggerExampleSerializer.FromObject(body);
    }

    private static void SetJsonExamples(
        OpenApiOperation operation,
        Dictionary<string, (string Summary, string Description, object Body)> examples)
    {
        if (!TryGetJsonMediaType(operation, out var mediaType))
        {
            return;
        }

        mediaType.Example = null;
        mediaType.Examples = examples.ToDictionary(
            entry => entry.Key,
            entry => new OpenApiExample
            {
                Summary = entry.Value.Summary,
                Description = entry.Value.Description,
                Value = SwaggerExampleSerializer.FromObject(entry.Value.Body),
            });
    }

    private static bool TryGetJsonMediaType(OpenApiOperation operation, out OpenApiMediaType mediaType)
    {
        mediaType = null!;
        return operation.RequestBody?.Content is not null
               && operation.RequestBody.Content.TryGetValue("application/json", out mediaType!);
    }

    private static void AppendDescription(OpenApiOperation operation, string note)
    {
        operation.Description = string.IsNullOrWhiteSpace(operation.Description)
            ? note
            : $"{operation.Description}\n\n{note}";
    }
}
