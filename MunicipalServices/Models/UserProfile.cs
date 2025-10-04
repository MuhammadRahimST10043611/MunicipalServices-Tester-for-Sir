using System.ComponentModel.DataAnnotations;

namespace MunicipalServices.Models
{
    public class UserProfile
    {
        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Your Name")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = "";
    }
}