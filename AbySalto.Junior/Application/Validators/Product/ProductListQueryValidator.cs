using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Application.DTOs.Product;
using FluentValidation;

namespace AbySalto.Junior.Application.Validators.Product;

public class ProductListQueryValidator : AbstractValidator<ProductListQuery>
{
    public ProductListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Page.HasValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, Pagination.MaxPageSize)
            .When(x => x.PageSize.HasValue);

        RuleFor(x => x.SortBy)
            .Must(sortBy => ProductSortFields.Allowed.Contains(sortBy!.Trim()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", ProductSortFields.Allowed.OrderBy(field => field))}.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortDirection)
            .IsInEnum()
            .When(x => x.SortDirection.HasValue);
    }
}
