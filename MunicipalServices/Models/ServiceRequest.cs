using System.ComponentModel.DataAnnotations;

namespace MunicipalServices.Models
{
    public class ServiceRequest : IIdentifiable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location")]
        public string Location { get; set; } = "";

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public string Category { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = "";

        [Display(Name = "Attached Files")]
        public CustomLinkedList<string> AttachedFiles { get; set; } = new CustomLinkedList<string>();

        public DateTime DateReported { get; set; }
        public string Status { get; set; } = "Submitted";
        public int Priority { get; set; } = 1;

        // Track which user created this report
        public int UserId { get; set; }
    }
}