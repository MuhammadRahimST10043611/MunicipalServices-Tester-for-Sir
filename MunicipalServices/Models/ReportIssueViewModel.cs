using System.ComponentModel.DataAnnotations;

namespace MunicipalServices.Models
{
    public class ReportIssueViewModel
    {
        [Required(ErrorMessage = "Location is required")]
        [Display(Name = "Location of Issue")]
        public string Location { get; set; } = "";

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Issue Category")]
        public string Category { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Detailed Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = "";

        // CHANGED: Use custom collection instead of ICollection<IFormFile>
        [Display(Name = "Attach Files (Images/Documents)")]
        public CustomCollection<IFormFile>? AttachedFiles { get; set; }

        public CustomLinkedList<IFormFile> GetAttachedFilesAsCustomList()
        {
            var customList = new CustomLinkedList<IFormFile>();
            if (AttachedFiles != null)
            {
                return AttachedFiles.ToCustomLinkedList();
            }
            return customList;
        }

        private static readonly CustomLinkedList<string> _categories;

        static ReportIssueViewModel()
        {
            _categories = new CustomLinkedList<string>();
            _categories.Add("Water & Sewer");
            _categories.Add("Roads & Transportation");
            _categories.Add("Electricity");
            _categories.Add("Waste Management");
            _categories.Add("Parks & Recreation");
            _categories.Add("Public Safety");
            _categories.Add("Building & Planning");
            _categories.Add("Other");
        }

        public CustomLinkedList<string> Categories => _categories;

        // CHANGED: Return custom array instead of string[]
        public CustomArray<string> GetCategoriesArray()
        {
            var array = new CustomArray<string>();
            array.CopyFrom(_categories);
            return array;
        }
    }
}