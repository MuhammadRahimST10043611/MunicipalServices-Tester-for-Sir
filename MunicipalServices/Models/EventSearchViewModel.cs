namespace MunicipalServices.Models
{
    public class EventSearchViewModel
    {
        public string SearchTerm { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; } = "date";

        // Results
        public CustomLinkedList<LocalEvent> Events { get; set; } = new CustomLinkedList<LocalEvent>();
        public CustomLinkedList<LocalEvent> RecommendedEvents { get; set; } = new CustomLinkedList<LocalEvent>();
        public CustomLinkedList<string> Categories { get; set; } = new CustomLinkedList<string>();
        public CustomLinkedList<string> RecentSearches { get; set; } = new CustomLinkedList<string>();

        // Statistics
        public int TotalEvents { get; set; }
        public int FilteredEvents { get; set; }
    }
}