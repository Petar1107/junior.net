using AbySalto.Junior.Application.DTOs.Product;
using AbySalto.Junior.Domain.Entities;
using AutoMapper;

namespace AbySalto.Junior.Application.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponse>();

        CreateMap<CreateProductRequest, Product>()
            .IgnoreIdAndAuditFields();

        CreateMap<UpdateProductRequest, Product>()
            .IgnoreIdAndAuditFields();
    }
}
