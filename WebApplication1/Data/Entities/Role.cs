namespace Candle_API.Data.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }    // "Admin", "User", etc.
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
