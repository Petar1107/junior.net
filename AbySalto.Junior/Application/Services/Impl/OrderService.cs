using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Application.DTOs.Common;
using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.Exceptions;
using AbySalto.Junior.Domain.Entities;
using AbySalto.Junior.Domain.Entities.Identity;
using AbySalto.Junior.Domain.Enums;
using AbySalto.Junior.Infrastructure.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Application.Services.Impl;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderRequest> _createValidator;
    private readonly IValidator<UpdateOrderRequest> _updateValidator;
    private readonly IValidator<CreateOrderItemRequest> _createItemValidator;
    private readonly IValidator<UpdateOrderItemRequest> _updateItemValidator;
    private readonly IValidator<OrderListQuery> _listQueryValidator;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IValidator<CreateOrderRequest> createValidator,
        IValidator<UpdateOrderRequest> updateValidator,
        IValidator<CreateOrderItemRequest> createItemValidator,
        IValidator<UpdateOrderItemRequest> updateItemValidator,
        IValidator<OrderListQuery> listQueryValidator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _currentUser = currentUser;
        _userManager = userManager;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _createItemValidator = createItemValidator;
        _updateItemValidator = updateItemValidator;
        _listQueryValidator = listQueryValidator;
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        NormalizeCreateRequest(request);
        await ValidateAsync(_createValidator, request, cancellationToken);

        if (HasDuplicateProducts(request.Items))
        {
            throw new BadRequestException("Each product can only appear once per order.");
        }

        var (customerId, customerName) = await ResolveCustomerAsync(request, cancellationToken);

        var order = new Order
        {
            CustomerId = customerId,
            CustomerName = customerName,
            Status = OrderStatus.Pending,
            OrderedAt = DateTimeOffset.UtcNow,
            PaymentMethod = request.PaymentMethod,
            ContactPhone = request.ContactPhone,
            DeliveryAddress = request.DeliveryAddress,
            Note = request.Note,
            Currency = request.Currency,
        };

        foreach (var itemRequest in request.Items)
        {
            var product = await GetAvailableProductAsync(itemRequest.ProductId, itemRequest.Quantity, cancellationToken);
            order.Items.Add(CreateOrderItem(itemRequest, product));
            product.UnitsInStock -= itemRequest.Quantity;
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{id}' was not found.");

        EnsureCanAccessOrder(order);

        return _mapper.Map<OrderResponse>(order);
    }

    public async Task<PagedResponse<OrderListItemResponse>> GetAllAsync(
        OrderListQuery query,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_listQueryValidator, query, cancellationToken);

        var (page, pageSize) = Pagination.Normalize(query);

        IQueryable<Order> ordersQuery = _orderRepository.Query()
            .AsNoTracking()
            .Include(order => order.Items);

        ordersQuery = ApplyCustomerScope(ordersQuery);

        if (query.Status.HasValue)
        {
            ordersQuery = ordersQuery.Where(order => order.Status == query.Status.Value);
        }

        var totalCount = await ordersQuery.CountAsync(cancellationToken);

        var orders = await ApplySorting(ordersQuery, query)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<OrderListItemResponse>
        {
            Items = _mapper.Map<IReadOnlyList<OrderListItemResponse>>(orders),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<OrderResponse> UpdateAsync(
        Guid id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        NormalizeUpdateRequest(request);
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var order = await _orderRepository.GetByIdWithItemsAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{id}' was not found.");

        EnsureCanAccessOrder(order);

        if (HasHeaderUpdates(request))
        {
            EnsureOrderPending(order);
            ApplyHeaderUpdates(order, request);
        }

        if (request.Status.HasValue)
        {
            EnsureAdmin();
            ApplyStatusChange(order, request.Status.Value);
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    public async Task<OrderResponse> AddItemAsync(
        Guid orderId,
        CreateOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createItemValidator, request, cancellationToken);

        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{orderId}' was not found.");

        EnsureCanAccessOrder(order);
        EnsureOrderPending(order);

        if (order.Items.Any(item => item.ProductId == request.ProductId))
        {
            throw new BadRequestException("This product is already on the order.");
        }

        var product = await GetAvailableProductAsync(request.ProductId, request.Quantity, cancellationToken);
        order.Items.Add(CreateOrderItem(request, product));
        product.UnitsInStock -= request.Quantity;

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    public async Task<OrderResponse> UpdateItemAsync(
        Guid orderId,
        Guid itemId,
        UpdateOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateItemValidator, request, cancellationToken);

        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{orderId}' was not found.");

        EnsureCanAccessOrder(order);
        EnsureOrderPending(order);

        var item = order.Items.FirstOrDefault(orderItem => orderItem.Id == itemId)
            ?? throw new NotFoundException($"Order item with id '{itemId}' was not found.");

        var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{item.ProductId}' was not found.");

        var newQuantity = request.Quantity ?? item.Quantity;
        var quantityDelta = newQuantity - item.Quantity;

        if (quantityDelta > 0)
        {
            EnsureProductAvailable(product, quantityDelta);
            product.UnitsInStock -= quantityDelta;
        }
        else if (quantityDelta < 0)
        {
            product.UnitsInStock += Math.Abs(quantityDelta);
        }

        if (request.Quantity.HasValue)
        {
            item.Quantity = newQuantity;
        }

        if (request.UnitPrice.HasValue)
        {
            item.UnitPrice = request.UnitPrice.Value;
        }

        if (request.DiscountPercentage.HasValue)
        {
            item.DiscountPercentage = request.DiscountPercentage.Value;
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    public async Task<OrderResponse> RemoveItemAsync(
        Guid orderId,
        Guid itemId,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{orderId}' was not found.");

        EnsureCanAccessOrder(order);
        EnsureOrderPending(order);

        var item = order.Items.FirstOrDefault(orderItem => orderItem.Id == itemId)
            ?? throw new NotFoundException($"Order item with id '{itemId}' was not found.");

        if (order.Items.Count <= 1)
        {
            throw new BadRequestException("An order must contain at least one item.");
        }

        var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
        if (product is not null)
        {
            product.UnitsInStock += item.Quantity;
        }

        _orderRepository.RemoveItem(item);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return await MapOrderResponseAsync(order.Id, cancellationToken);
    }

    private async Task<OrderResponse> MapOrderResponseAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order with id '{orderId}' was not found.");

        return _mapper.Map<OrderResponse>(order);
    }

    private IQueryable<Order> ApplyCustomerScope(IQueryable<Order> query)
    {
        if (IsAdmin())
        {
            return query;
        }

        if (!_currentUser.UserId.HasValue)
        {
            throw new ForbiddenException("You are not allowed to access this order.");
        }

        return query.Where(order => order.CustomerId == _currentUser.UserId.Value);
    }

    private void EnsureCanAccessOrder(Order order)
    {
        if (IsAdmin())
        {
            return;
        }

        if (!_currentUser.UserId.HasValue || order.CustomerId != _currentUser.UserId.Value)
        {
            throw new ForbiddenException("You are not allowed to access this order.");
        }
    }

    private static void EnsureOrderPending(Order order)
    {
        if (order.Status != OrderStatus.Pending)
        {
            throw new BadRequestException("Order can only be modified while it is pending.");
        }
    }

    private void EnsureAdmin()
    {
        if (!IsAdmin())
        {
            throw new ForbiddenException("Only administrators can change the order status.");
        }
    }

    private static void ApplyStatusChange(Order order, OrderStatus newStatus)
    {
        if (order.Status == OrderStatus.Completed)
        {
            throw new BadRequestException("Completed orders cannot be updated.");
        }

        if (!IsValidStatusTransition(order.Status, newStatus))
        {
            throw new BadRequestException(
                $"Cannot change order status from '{order.Status}' to '{newStatus}'.");
        }

        order.Status = newStatus;
    }
    
    private static bool IsValidStatusTransition(OrderStatus current, OrderStatus next) =>
        (current, next) is
        (OrderStatus.Pending, OrderStatus.InPreparation)
        or (OrderStatus.InPreparation, OrderStatus.Completed);

    private void ApplyHeaderUpdates(Order order, UpdateOrderRequest request)
    {
        if (request.CustomerName is not null)
        {
            if (!IsAdmin())
            {
                throw new ForbiddenException("Only administrators can change the customer name.");
            }

            order.CustomerName = request.CustomerName.Trim();
        }

        if (request.PaymentMethod.HasValue)
        {
            order.PaymentMethod = request.PaymentMethod.Value;
        }

        if (request.ContactPhone is not null)
        {
            order.ContactPhone = request.ContactPhone.Trim();
        }

        if (request.DeliveryAddress is not null)
        {
            order.DeliveryAddress = NormalizeOptionalText(request.DeliveryAddress);
        }

        if (request.Note is not null)
        {
            order.Note = NormalizeOptionalText(request.Note);
        }

        if (request.Currency is not null)
        {
            order.Currency = request.Currency.Trim().ToUpperInvariant();
        }
    }

    private static bool HasHeaderUpdates(UpdateOrderRequest request) =>
        request.CustomerName is not null
        || request.PaymentMethod.HasValue
        || request.ContactPhone is not null
        || request.DeliveryAddress is not null
        || request.Note is not null
        || request.Currency is not null;

    private async Task<(Guid? CustomerId, string CustomerName)> ResolveCustomerAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (IsAdmin())
        {
            var customerId = request.CustomerId;
            var customerName = NormalizeOptionalText(request.CustomerName);

            if (customerId.HasValue)
            {
                EnsureValidCustomerId(customerId.Value);
                var resolvedName = await ResolveCustomerNameFromUserAsync(customerId.Value);
                customerName ??= resolvedName;
            }

            return (customerId, customerName ?? "Guest");
        }

        if (!_currentUser.UserId.HasValue)
        {
            throw new ForbiddenException("You must be signed in to create an order.");
        }

        if (request.CustomerId.HasValue && request.CustomerId.Value != _currentUser.UserId.Value)
        {
            throw new ForbiddenException("You cannot create an order for another customer.");
        }

        var name = NormalizeOptionalText(request.CustomerName)
            ?? _currentUser.Email
            ?? "Customer";

        return (_currentUser.UserId.Value, name);
    }

    private static void EnsureValidCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new BadRequestException("CustomerId must be a valid non-empty GUID.");
        }
    }

    private async Task<string> ResolveCustomerNameFromUserAsync(Guid customerId)
    {
        var user = await _userManager.FindByIdAsync(customerId.ToString());
        if (user is null)
        {
            throw new BadRequestException($"Customer with id '{customerId}' was not found.");
        }

        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? user.Email ?? "Customer" : fullName;
    }

    private async Task<Product> GetAvailableProductAsync(
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{productId}' was not found.");

        EnsureProductAvailable(product, quantity);
        return product;
    }

    private static void EnsureProductAvailable(Product product, int quantity)
    {
        if (!product.IsActive)
        {
            throw new BadRequestException($"Product '{product.Name}' is not available.");
        }

        if (product.UnitsInStock < quantity)
        {
            throw new BadRequestException(
                $"Insufficient stock for product '{product.Name}'. Available: {product.UnitsInStock}, requested: {quantity}.");
        }
    }

    private static OrderItem CreateOrderItem(CreateOrderItemRequest request, Product product) =>
        new()
        {
            ProductId = product.Id,
            Product = product,
            UnitPrice = request.UnitPrice ?? product.Price,
            Quantity = request.Quantity,
            DiscountPercentage = request.DiscountPercentage ?? 0m,
        };

    private static bool HasDuplicateProducts(IEnumerable<CreateOrderItemRequest> items) =>
        items.GroupBy(item => item.ProductId).Any(group => group.Count() > 1);

    private static IQueryable<Order> ApplySorting(IQueryable<Order> query, OrderListQuery listQuery)
    {
        var sortBy = OrderSortFields.GetSortByOrDefault(listQuery.SortBy);
        var descending = listQuery.SortDirection == SortDirection.Desc;

        if (OrderSortFields.Is(sortBy, OrderSortFields.TotalAmount))
        {
            return descending
                ? query.OrderByDescending(order => order.Items.Sum(item =>
                    item.UnitPrice * item.Quantity * (1 - item.DiscountPercentage / 100m)))
                : query.OrderBy(order => order.Items.Sum(item =>
                    item.UnitPrice * item.Quantity * (1 - item.DiscountPercentage / 100m)));
        }

        if (OrderSortFields.Is(sortBy, OrderSortFields.Status))
        {
            return descending
                ? query.OrderByDescending(order => order.Status)
                : query.OrderBy(order => order.Status);
        }

        if (OrderSortFields.Is(sortBy, OrderSortFields.CustomerName))
        {
            return descending
                ? query.OrderByDescending(order => order.CustomerName)
                : query.OrderBy(order => order.CustomerName);
        }

        if (OrderSortFields.Is(sortBy, OrderSortFields.CreatedOn))
        {
            return descending
                ? query.OrderByDescending(order => order.CreatedOn)
                : query.OrderBy(order => order.CreatedOn);
        }

        return descending
            ? query.OrderByDescending(order => order.OrderedAt)
            : query.OrderBy(order => order.OrderedAt);
    }

    private bool IsAdmin() =>
        _currentUser.IsInRole(UserRoleNames.From(UserRole.Admin));

    private static void NormalizeCreateRequest(CreateOrderRequest request)
    {
        request.CustomerName = NormalizeOptionalText(request.CustomerName);
        request.ContactPhone = request.ContactPhone.Trim();
        request.DeliveryAddress = NormalizeOptionalText(request.DeliveryAddress);
        request.Note = NormalizeOptionalText(request.Note);
        request.Currency = request.Currency.Trim().ToUpperInvariant();
    }

    private static void NormalizeUpdateRequest(UpdateOrderRequest request)
    {
        if (request.CustomerName is not null)
        {
            request.CustomerName = request.CustomerName.Trim();
        }

        if (request.ContactPhone is not null)
        {
            request.ContactPhone = request.ContactPhone.Trim();
        }

        if (request.DeliveryAddress is not null)
        {
            request.DeliveryAddress = NormalizeOptionalText(request.DeliveryAddress);
        }

        if (request.Note is not null)
        {
            request.Note = NormalizeOptionalText(request.Note);
        }

        if (request.Currency is not null)
        {
            request.Currency = request.Currency.Trim().ToUpperInvariant();
        }
    }

    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
