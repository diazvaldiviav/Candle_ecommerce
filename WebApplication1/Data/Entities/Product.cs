namespace Candle_API.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SubcategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public SubCategory? SubCategory { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<ProductColor>? ProductColors { get; set; }
        public ICollection<ProductSize>? ProductSizes { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ProductAroma>? ProductAromas { get; set; }
    }
}
