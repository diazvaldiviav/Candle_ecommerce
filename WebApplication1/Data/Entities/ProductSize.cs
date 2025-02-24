namespace Candle_API.Data.Entities
{
    public class ProductSize
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int AddicionalPrice { get; set; } = 0;

        public int Stock { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Size Size { get; set; }
    }
}
