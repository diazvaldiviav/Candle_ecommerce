namespace Candle_API.Data.DTOs.Aromas
{
    public class AromaDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // DTOs/Aroma/CreateAromaDTO.cs
    public class CreateAromaDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // DTOs/Aroma/AddAromaToProductDTO.cs
    public class AddAromaToProductDTO
    {
        public int AromaId { get; set; }
        public decimal AdditionalPrice { get; set; }
    }

    public class ProductAromaDTO
    {
        public int ProductId { get; set; }
        public int AromaId { get; set; }
        public decimal AdditionalPrice { get; set; }

        // Propiedades de navegación
        public string? ProductName { get; set; }
        public string? AromaName { get; set; }
    }
}
