namespace MunicipalServices.Models
{
    public class UserSearchHistory
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Nullable for anonymous users
        public string SearchTerm { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime SearchDate { get; set; } = DateTime.UtcNow;
        public string SessionId { get; set; } = ""; // For anonymous users

        // Navigation property
        public User? User { get; set; }
    }
}