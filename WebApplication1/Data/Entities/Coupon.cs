namespace Candle_API.Data.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinimumPurchase { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int MaxUses { get; set; }
        public int CurrentUses { get; set; }
    }
}
