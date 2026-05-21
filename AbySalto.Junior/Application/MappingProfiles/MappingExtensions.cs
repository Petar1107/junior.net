using AbySalto.Junior.Domain.Common;
using AutoMapper;

namespace AbySalto.Junior.Application.MappingProfiles;

internal static class MappingExtensions
{
    public static IMappingExpression<TSource, TDestination> IgnoreIdAndAuditFields<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
        where TDestination : BaseEntity
    {
        return expression
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());
    }
}
