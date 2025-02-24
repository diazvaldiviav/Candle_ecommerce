namespace Candle_API.Data.Entities
{
    public class ProductAroma
    {
        public int ProductId { get; set; }
        public int AromaId { get; set; }
        public decimal AdditionalPrice { get; set; } = 0; // Precio adicional por este aroma

        // Navigation properties
        public Product? Product { get; set; }
        public Aroma? Aroma { get; set; }
    }
}
