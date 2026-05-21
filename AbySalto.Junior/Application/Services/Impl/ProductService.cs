using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Application.DTOs.Common;
using AbySalto.Junior.Application.DTOs.Product;
using AbySalto.Junior.Application.Exceptions;
using AbySalto.Junior.Domain.Entities;
using AbySalto.Junior.Infrastructure.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Application.Services.Impl;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;
    private readonly IValidator<ProductListQuery> _listQueryValidator;

    public ProductService(
        IProductRepository repository,
        IMapper mapper,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator,
        IValidator<ProductListQuery> listQueryValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _listQueryValidator = listQueryValidator;
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        NormalizeCreateRequest(request);
        await ValidateAsync(_createValidator, request, cancellationToken);

        if (await _repository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
        {
            throw new ConflictException("A product with this name already exists.");
        }

        var product = _mapper.Map<Product>(request);
        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Product with id '{id}' was not found.");
        }

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(
        ProductListQuery query,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_listQueryValidator, query, cancellationToken);

        var (page, pageSize) = Pagination.Normalize(query);

        var productsQuery = _repository.Query().AsNoTracking();

        if (query.IsActive.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.IsActive == query.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchTerm = query.Search.Trim().ToLower();
            productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(searchTerm));
        }

        var totalCount = await productsQuery.CountAsync(cancellationToken);

        var products = await ApplySorting(productsQuery, query)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductResponse>
        {
            Items = _mapper.Map<IReadOnlyList<ProductResponse>>(products),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, ProductListQuery listQuery)
    {
        var sortBy = ProductSortFields.GetSortByOrDefault(listQuery.SortBy);
        var descending = listQuery.SortDirection == SortDirection.Desc;

        if (ProductSortFields.Is(sortBy, ProductSortFields.Price))
        {
            return descending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price);
        }

        if (ProductSortFields.Is(sortBy, ProductSortFields.UnitsInStock))
        {
            return descending
                ? query.OrderByDescending(p => p.UnitsInStock)
                : query.OrderBy(p => p.UnitsInStock);
        }

        if (ProductSortFields.Is(sortBy, ProductSortFields.IsActive))
        {
            return descending
                ? query.OrderByDescending(p => p.IsActive)
                : query.OrderBy(p => p.IsActive);
        }

        if (ProductSortFields.Is(sortBy, ProductSortFields.CreatedOn))
        {
            return descending
                ? query.OrderByDescending(p => p.CreatedOn)
                : query.OrderBy(p => p.CreatedOn);
        }

        return descending
            ? query.OrderByDescending(p => p.Name)
            : query.OrderBy(p => p.Name);
    }

    public async Task<ProductResponse> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        NormalizeUpdateRequest(request);
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{id}' was not found.");

        if (await _repository.ExistsByNameAsync(request.Name, id, cancellationToken))
        {
            throw new ConflictException("A product with this name already exists.");
        }

        _mapper.Map(request, product);
        await _repository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{id}' was not found.");

        if (!product.IsActive)
        {
            return;
        }

        product.IsActive = false;
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeCreateRequest(CreateProductRequest request)
    {
        request.Name = request.Name.Trim();
        request.Description = NormalizeDescription(request.Description);
    }

    private static void NormalizeUpdateRequest(UpdateProductRequest request)
    {
        request.Name = request.Name.Trim();
        request.Description = NormalizeDescription(request.Description);
    }

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    private static async Task ValidateAsync<T>(
        IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (result.IsValid)
        {
            return;
        }

        var message = string.Join("; ", result.Errors.Select(error => error.ErrorMessage));
        throw new BadRequestException(message);
    }
}
