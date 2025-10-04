using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MunicipalServices.Models
{
    public class LocalEvent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Event description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Event category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = "";

        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string Location { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // For priority in recommendation system
        public int Priority { get; set; } = 1;

        // For tracking user interactions
        public int ViewCount { get; set; } = 0;
    }
}