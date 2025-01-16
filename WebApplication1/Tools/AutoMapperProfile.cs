// Mappings/AutoMapperProfile.cs
using AutoMapper;
using Candle_API.Data.DTOs.Categories;
using Candle_API.Data.DTOs.Product;
using Candle_API.Data.Entities;


namespace Candle_API.Tools;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        //mapping for Category
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<SubCategory, SubcategoryDto>();



        //mapping for Product
        CreateMap<Product, ProductDto>()
         .ForMember(dest => dest.SubcategoryName,
                   opt => opt.MapFrom(src => src.SubCategory.Name));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<ProductColor, ProductColorDto>()
            .ForMember(dest => dest.ColorName,
                      opt => opt.MapFrom(src => src.Color.Name))
            .ForMember(dest => dest.HexCode,
                      opt => opt.MapFrom(src => src.Color.HexCode));
        CreateMap<ProductSize, ProductSizeDto>()
            .ForMember(dest => dest.SizeName,
                      opt => opt.MapFrom(src => src.Size.Name));



    }
}