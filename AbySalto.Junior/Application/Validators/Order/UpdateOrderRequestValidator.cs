using AbySalto.Junior.Application.DTOs.Order;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Order;

public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Status.HasValue
                || x.CustomerName is not null
                || x.PaymentMethod.HasValue
                || x.ContactPhone is not null
                || x.DeliveryAddress is not null
                || x.Note is not null
                || x.Currency is not null)
            .WithMessage("At least one field must be provided.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => x.CustomerName is not null);

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .When(x => x.PaymentMethod.HasValue);

        RuleFor(x => x.ContactPhone)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.ContactPhone is not null);

        RuleFor(x => x.DeliveryAddress)
            .MaximumLength(500)
            .When(x => x.DeliveryAddress is not null);

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .When(x => x.Note is not null);

        RuleFor(x => x.Currency)
            .Length(3)
            .When(x => x.Currency is not null);
    }
}
