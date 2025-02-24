namespace Candle_API.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Para relacionar con el usuario
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
