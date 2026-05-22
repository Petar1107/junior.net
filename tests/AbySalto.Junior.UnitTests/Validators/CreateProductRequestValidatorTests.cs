using AbySalto.Junior.Application.DTOs.Product;
using AbySalto.Junior.Application.Validators.Product;

namespace AbySalto.Junior.UnitTests.Validators;

public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Price = 10m,
            UnitsInStock = 5,
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_price_is_zero()
    {
        var request = new CreateProductRequest
        {
            Name = "Pizza",
            Price = 0m,
            UnitsInStock = 5,
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_not_have_errors_for_valid_request()
    {
        var request = new CreateProductRequest
        {
            Name = "Pizza",
            Price = 12.50m,
            UnitsInStock = 20,
            IsActive = true,
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
