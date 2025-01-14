using System.ComponentModel.DataAnnotations;

namespace Candle_API.Data.DTOs.Categories
{
    public class SubCategorieDTOs
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]

        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    // DTOs/CategoryDtos.cs
    public class UpdateSubcategoryDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }
    }
}
