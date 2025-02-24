// Mappings/AutoMapperProfile.cs
using AutoMapper;
using Candle_API.Data.DTOs.Aromas;
using Candle_API.Data.DTOs.Cart;
using Candle_API.Data.DTOs.Categories;
using Candle_API.Data.DTOs.Colors;
using Candle_API.Data.DTOs.Product;
using Candle_API.Data.DTOs.Size;
using Candle_API.Data.DTOs.SubCategories;
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

        CreateMap<CreateProductDto, Product>()
           .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src =>
               new List<ProductImage>
               {
                    new ProductImage
                    {
                        ImageUrl = src.ImageUrl,
                        IsMain = true
                    }
               }))
           .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<Product, ProductResponseDTO>()
           .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

        CreateMap<ProductImage, ProductImageDto>();


        CreateMap<UpdateProductDto, Product>();
        CreateMap<ProductColor, ProductColorDto>()
            .ForMember(dest => dest.ColorName,
                      opt => opt.MapFrom(src => src.Color.Name))
            .ForMember(dest => dest.HexCode,
                      opt => opt.MapFrom(src => src.Color.HexCode));
           
        CreateMap<ProductSize, ProductSizeDto>()
            .ForMember(dest => dest.SizeName,
                      opt => opt.MapFrom(src => src.Size.Name));

        CreateMap<SubCategory, SubCategoryDto>()
        .ForMember(dest => dest.CategoryName,
                  opt => opt.MapFrom(src => src.Category.Name));


        CreateMap<Color, ColorDto>();

        CreateMap<Size, SizeDto>();


        CreateMap<Aroma, AromaDTO>();
        CreateMap<CreateAromaDTO, Aroma>();
        CreateMap<ProductAroma, ProductAromaDTO>();


        CreateMap<Cart, CartDTO>()
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
           .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
           .ForMember(dest => dest.Tax, opt => opt.Ignore())
           .ForMember(dest => dest.Total, opt => opt.Ignore())
           .ForMember(dest => dest.ItemCount, opt => opt.Ignore());

        CreateMap<CartItem, CartItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImage, opt =>
                opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault().ImageUrl))
            .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color.Name))
            .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Size.Name))
            .ForMember(dest => dest.AromaName, opt => opt.MapFrom(src => src.Aroma.Name))
            .ForMember(dest => dest.Subtotal, opt =>
                opt.MapFrom(src => src.Quantity * src.UnitPrice));



    }
}