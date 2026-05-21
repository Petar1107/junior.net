using AbySalto.Junior.Application.Common;
using AbySalto.Junior.Application.DTOs.Order;
using AbySalto.Junior.Domain.Entities;
using AutoMapper;

namespace AbySalto.Junior.Application.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => OrderCalculations.CalculateOrderTotal(src.Items)))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<Order, OrderListItemResponse>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => OrderCalculations.CalculateOrderTotal(src.Items)));

        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => OrderCalculations.CalculateItemTotal(src)));
    }
}
