using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.Validators.Order;
using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.UnitTests.Validators;

public class CreateOrderRequestValidatorTests
{
    private readonly CreateOrderRequestValidator _validator = new();

    [Fact]
    public void Should_require_customer_name_for_guest_order()
    {
        var request = new CreateOrderRequest
        {
            PaymentMethod = PaymentMethod.Card,
            ContactPhone = "+385911234567",
            Currency = "EUR",
            Items = [new CreateOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 }],
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void Should_have_error_when_items_are_empty()
    {
        var request = new CreateOrderRequest
        {
            CustomerName = "Guest",
            PaymentMethod = PaymentMethod.Cash,
            ContactPhone = "+385911234567",
            Currency = "EUR",
            Items = [],
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Should_not_have_errors_for_valid_guest_order()
    {
        var request = new CreateOrderRequest
        {
            CustomerName = "Table 3",
            PaymentMethod = PaymentMethod.Card,
            ContactPhone = "+385911234567",
            Currency = "EUR",
            Items = [new CreateOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 }],
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
