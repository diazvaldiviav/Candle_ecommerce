// Mappings/AutoMapperProfile.cs
using AutoMapper;
using Candle_API.Data.DTOs.Categories;
using Candle_API.Data.Entities;


namespace Candle_API.Tools;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<SubCategory, SubcategoryDto>();
    }
}