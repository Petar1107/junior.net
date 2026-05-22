using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.Validators.Order;
using AbySalto.Junior.Domain.Enums;

namespace AbySalto.Junior.UnitTests.Validators;

public class UpdateOrderRequestValidatorTests
{
    private readonly UpdateOrderRequestValidator _validator = new();

    [Fact]
    public void Should_have_error_when_no_fields_provided()
    {
        var request = new UpdateOrderRequest();

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Should_not_have_errors_when_only_status_is_provided()
    {
        var request = new UpdateOrderRequest { Status = OrderStatus.InPreparation };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_have_error_when_currency_is_not_three_characters()
    {
        var request = new UpdateOrderRequest { Currency = "EURO" };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }
}
