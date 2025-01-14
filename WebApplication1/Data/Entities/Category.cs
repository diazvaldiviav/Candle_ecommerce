namespace Candle_API.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ImageUrl { get; set; }

        // Navigation properties
        public ICollection<SubCategory>? Subcategories { get; set; }

    }
}
