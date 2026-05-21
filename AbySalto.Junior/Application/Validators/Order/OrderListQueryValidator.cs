using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Application.DTOs.Order;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Order;

public class OrderListQueryValidator : AbstractValidator<OrderListQuery>
{
    public OrderListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Page.HasValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, Pagination.MaxPageSize)
            .When(x => x.PageSize.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.SortBy)
            .Must(sortBy => OrderSortFields.Allowed.Contains(sortBy!.Trim()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", OrderSortFields.Allowed.OrderBy(field => field))}.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortDirection)
            .IsInEnum()
            .When(x => x.SortDirection.HasValue);
    }
}
