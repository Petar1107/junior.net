using AbySalto.Junior.Application.DTOs.Order;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Order;

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue);
    }
}
