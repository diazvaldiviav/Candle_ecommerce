namespace Candle_API.Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public int ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}
