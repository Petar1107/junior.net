using AbySalto.Junior.Application.DTOs.Product;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Product;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo(0);
    }
}
