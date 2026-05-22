using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Application.Exceptions;
using AbySalto.Junior.Application.Services;
using AbySalto.Junior.Application.Services.Impl;
using AbySalto.Junior.Application.Validators.Order;
using AbySalto.Junior.Domain.Entities;
using AbySalto.Junior.Domain.Entities.Identity;
using AbySalto.Junior.Domain.Enums;
using AbySalto.Junior.Infrastructure.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AbySalto.Junior.UnitTests.Services;

public class OrderServiceCreateTests
{
    [Fact]
    public async Task CreateAsync_WhenQuantityExceedsUnitsInStock_ThrowsBadRequest()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Margherita",
            Price = 12.50m,
            UnitsInStock = 2,
            IsActive = true,
        };

        var productRepository = new Mock<IProductRepository>();
        productRepository
            .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser
            .Setup(user => user.IsInRole(UserRoleNames.From(UserRole.Admin)))
            .Returns(true);

        var orderRepository = new Mock<IOrderRepository>();
        var mapper = new Mock<IMapper>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var sut = new OrderService(
            orderRepository.Object,
            productRepository.Object,
            currentUser.Object,
            userManager.Object,
            mapper.Object,
            new CreateOrderRequestValidator(),
            new UpdateOrderRequestValidator(),
            new CreateOrderItemRequestValidator(),
            new UpdateOrderItemRequestValidator(),
            new OrderListQueryValidator());

        var request = new CreateOrderRequest
        {
            CustomerName = "Table 5",
            PaymentMethod = PaymentMethod.Card,
            ContactPhone = "+385911234567",
            Currency = "EUR",
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 5,
                },
            ],
        };

        var act = () => sut.CreateAsync(request, CancellationToken.None);

        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*Insufficient stock*Available: 2, requested: 5*");

        orderRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
