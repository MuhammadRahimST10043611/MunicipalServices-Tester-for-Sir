namespace MunicipalServices.Models
{
    public class UserEngagementStats
    {
        public string UserName { get; set; } = "Community Member";
        public int ReportsSubmitted { get; set; }
        public int CompletionPercentage { get; set; }
        public string CurrentLevel { get; set; } = "Community Starter";
        public string NextLevel { get; set; } = "Active Citizen";
        public int PointsToNextLevel { get; set; }
        public CustomLinkedList<string> Achievements { get; set; } = new CustomLinkedList<string>();
        public string MotivationalMessage { get; set; } = "Start making a difference in your community!";
        public bool ShowCelebration { get; set; } = false;
        public string CelebrationMessage { get; set; } = "";
    }
}
