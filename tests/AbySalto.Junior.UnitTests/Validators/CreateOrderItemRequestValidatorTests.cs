using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.Validators.Order;

namespace AbySalto.Junior.UnitTests.Validators;

public class CreateOrderItemRequestValidatorTests
{
    private readonly CreateOrderItemRequestValidator _validator = new();

    [Fact]
    public void Should_have_error_when_quantity_is_zero()
    {
        var request = new CreateOrderItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 0,
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Should_have_error_when_discount_exceeds_100()
    {
        var request = new CreateOrderItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            DiscountPercentage = 101m,
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.DiscountPercentage);
    }

    [Fact]
    public void Should_not_have_errors_for_valid_request()
    {
        var request = new CreateOrderItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            DiscountPercentage = 10m,
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
