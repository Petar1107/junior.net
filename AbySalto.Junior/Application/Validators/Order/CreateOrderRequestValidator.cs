using AbySalto.Junior.Application.DTOs.Order;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Order;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEqual(Guid.Empty)
            .When(x => x.CustomerId.HasValue);

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => !x.CustomerId.HasValue);

        RuleFor(x => x.PaymentMethod)
            .IsInEnum();

        RuleFor(x => x.ContactPhone)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.DeliveryAddress)
            .MaximumLength(500)
            .When(x => x.DeliveryAddress is not null);

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => x.Note is not null);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(x => x.Items)
            .NotEmpty();

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemRequestValidator());
    }
}
