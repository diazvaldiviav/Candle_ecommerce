namespace Candle_API.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public User User { get; set; }
        // public User User { get; set; }
    }
}
