using AbySalto.Junior.Application.DTOs.Common;
using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.Services;
using AbySalto.Junior.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbySalto.Junior.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private const string AdminAndCustomerRoles =
        $"{nameof(UserRole.Admin)},{nameof(UserRole.Customer)}";

    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(PagedResponse<OrderListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<OrderListItemResponse>>> GetAll(
        [FromQuery] OrderListQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Update(
        Guid id,
        [FromBody] UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/items")]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> AddItem(
        Guid id,
        [FromBody] CreateOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.AddItemAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/items/{itemId:guid}")]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> UpdateItem(
        Guid id,
        Guid itemId,
        [FromBody] UpdateOrderItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateItemAsync(id, itemId, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [Authorize(Roles = AdminAndCustomerRoles)]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> DeleteItem(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.RemoveItemAsync(id, itemId, cancellationToken);
        return Ok(result);
    }
}
