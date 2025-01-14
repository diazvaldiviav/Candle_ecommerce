namespace Candle_API.Data.Entities
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public ICollection<ProductSize> ProductSizes { get; set; }
    }
}
