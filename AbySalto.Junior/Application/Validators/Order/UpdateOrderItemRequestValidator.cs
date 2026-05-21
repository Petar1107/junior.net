using AbySalto.Junior.Application.DTOs.Order;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Order;

public class UpdateOrderItemRequestValidator : AbstractValidator<UpdateOrderItemRequest>
{
    public UpdateOrderItemRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Quantity.HasValue || x.UnitPrice.HasValue || x.DiscountPercentage.HasValue)
            .WithMessage("At least one field must be provided.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Quantity.HasValue);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue);
    }
}
