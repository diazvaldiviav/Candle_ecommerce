namespace Candle_API.Data.Entities
{
    public class ProductColor
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }

        public int Stock { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Color Color { get; set; }
    }
}
