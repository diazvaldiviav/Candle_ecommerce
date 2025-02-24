namespace Candle_API.Data.Entities
{
    public class Aroma
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property
        public ICollection<ProductAroma> ProductAromas { get; set; }
    }
}
