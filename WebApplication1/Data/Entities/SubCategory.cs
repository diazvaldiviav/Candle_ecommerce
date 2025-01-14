namespace Candle_API.Data.Entities
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
