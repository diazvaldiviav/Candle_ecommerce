namespace Candle_API.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }  // Guardamos el precio al momento de agregar al carrito

        // Propiedades opcionales para variantes del producto
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? AromaId { get; set; }

        // Navigation properties
        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public Color? Color { get; set; }
        public Size? Size { get; set; }
        public Aroma? Aroma { get; set; }
    }
}
