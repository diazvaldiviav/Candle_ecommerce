namespace Candle_API.Data.Entities
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HexCode { get; set; }

        // Navigation properties
        public ICollection<ProductColor> ProductColors { get; set; }
    }
}
