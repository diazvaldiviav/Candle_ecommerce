using System.Data;

namespace Candle_API.Data.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
