﻿using System.ComponentModel.DataAnnotations;

namespace Candle_API.Data.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProductColorDto> ProductColors { get; set; }
        public ICollection<ProductSizeDto> ProductSizes { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La subcategoría es requerida")]
        public int SubcategoryId { get; set; }

        [Required(ErrorMessage = "La URL de la imagen es requerida")]
        [Url(ErrorMessage = "Por favor ingrese una URL válida")]
        [RegularExpression(@"^https?:\/\/.*\.(png|jpg|jpeg|gif|webp)$",
        ErrorMessage = "La URL debe ser una imagen válida (png, jpg, jpeg, gif, webp)")]
        public string ImageUrl { get; set; }
    }


    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La URL de la imagen es requerida")]
        [Url(ErrorMessage = "Por favor ingrese una URL válida")]
        [RegularExpression(@"^https?:\/\/.*\.(png|jpg|jpeg|gif|webp)$",
       ErrorMessage = "La URL debe ser una imagen válida (png, jpg, jpeg, gif, webp)")]
        public string ImageUrl { get; set; }
    }

    public class ProductColorDto
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string HexCode { get; set; }
    }

    public class ProductSizeDto
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; }
    }

    //Crear size DTO
    public class CreateSizeDto
    {
        [Required(ErrorMessage = "El nombre del tamaño es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Name { get; set; }
    }

    //update size DTO
    public class UpdateSizeDto
    {
        [Required(ErrorMessage = "El nombre del tamaño es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Name { get; set; }
    }

    //Crear color
    public class CreateColorDto
    {
        [Required(ErrorMessage = "El nombre del color es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El código hexadecimal es requerido")]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$",
            ErrorMessage = "El código hexadecimal debe tener el formato #RRGGBB o #RGB")]
        public string HexCode { get; set; }
    }

    //update color
    // DTOs/ColorDto.cs
    public class UpdateColorDto
    {
        [Required(ErrorMessage = "El nombre del color es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El código hexadecimal es requerido")]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$",
            ErrorMessage = "El código hexadecimal debe tener el formato #RRGGBB o #RGB")]
        public string HexCode { get; set; }
    }


    //cambiar de categoria
    // DTOs/ProductDtos.cs
    public class ChangeSubcategoryDto
    {
        [Required(ErrorMessage = "El ID de la nueva subcategoría es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la subcategoría debe ser mayor que 0")]
        public int NewSubcategoryId { get; set; }
    }

    public class ProductSubcategoryChangeDto
    {
        public ProductDto Product { get; set; }
        public SubcategoryChangeInfo ChangeInfo { get; set; }
    }

    public class SubcategoryChangeInfo
    {
        public string OldCategory { get; set; }
        public string OldSubcategory { get; set; }
        public string NewCategory { get; set; }
        public string NewSubcategory { get; set; }
    }
}
