using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public class UserEngagementService : IUserEngagementService
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static string _userName = "";

        public UserEngagementService(IServiceRequestService serviceRequestService, IHttpContextAccessor httpContextAccessor)
        {
            _serviceRequestService = serviceRequestService;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") ?? 0;
        }

        public UserNameResult SetUserName(string name)
        {
            var result = new UserNameResult();
            var validation = ValidateUserName(name);

            if (!validation.Success)
            {
                result.Success = false;
                result.Message = validation.Message;
                return result;
            }

            _userName = name.Trim();
            result.Success = true;
            result.UserName = _userName;
            result.Message = $"Welcome {_userName}! Let's make a difference in your community together!";

            return result;
        }

        public string GetUserName()
        {
            return string.IsNullOrEmpty(_userName) ? "Community Member" : _userName;
        }

        public bool ShouldShowNameModal()
        {
            var currentUserName = GetUserName();
            return string.IsNullOrEmpty(_userName) || currentUserName == "Community Member";
        }

        public ServiceResult ValidateUserName(string name)
        {
            var result = new ServiceResult();

            if (string.IsNullOrWhiteSpace(name))
            {
                result.Success = false;
                result.Message = "Please enter a valid name.";
                return result;
            }

            if (name.Trim().Length < 2)
            {
                result.Success = false;
                result.Message = "Name must be at least 2 characters long.";
                return result;
            }

            if (name.Trim().Length > 50)
            {
                result.Success = false;
                result.Message = "Name cannot exceed 50 characters.";
                return result;
            }

            result.Success = true;
            return result;
        }

        public UserEngagementStats GetUserEngagementStats()
        {
            var userId = GetCurrentUserId();
            var userReports = _serviceRequestService.GetUserReportCount(userId);
            var userName = GetUserName();

            return new UserEngagementStats
            {
                UserName = userName,
                ReportsSubmitted = userReports,
                CompletionPercentage = CalculateCompletionPercentage(userReports),
                CurrentLevel = GetUserLevel(userReports),
                NextLevel = GetNextLevel(userReports),
                PointsToNextLevel = GetPointsToNextLevel(userReports),
                Achievements = GetAchievements(userReports),
                MotivationalMessage = GetMotivationalMessage(userReports, userName),
                ShowCelebration = ShouldShowCelebration(userReports),
                CelebrationMessage = GetCelebrationMessage(userReports, userName)
            };
        }

        public string GetMotivationalMessage(int reportsSubmitted, string userName)
        {
            return reportsSubmitted switch
            {
                0 => $"Welcome {userName}! Your first report will make a real difference in your community. We already have some existing reports from other community members - join them in making a change!",
                1 => $"Great start {userName}! You're already helping improve your neighborhood alongside other active citizens!",
                2 => $"Amazing work {userName}! One more report to become a Community Champion - you're making a real impact!",
                3 => $"Incredible {userName}! You've achieved Community Champion status!",
                _ => $"Outstanding {userName}! You're a true Community Hero making our area better!"
            };
        }

        public CustomLinkedList<string> GetAchievements(int reportsSubmitted)
        {
            var achievements = new CustomLinkedList<string>();

            if (reportsSubmitted >= 1)
                achievements.Add("First Report - Community Contributor");

            if (reportsSubmitted >= 3)
                achievements.Add("Community Champion - Three Reports Strong!");

            if (reportsSubmitted >= 5)
                achievements.Add("Community Hero - Five Reports Milestone");

            var categoryDiversityAchievement = CheckCategoryDiversityAchievement();
            if (!string.IsNullOrEmpty(categoryDiversityAchievement))
                achievements.Add(categoryDiversityAchievement);

            return achievements;
        }

        public string GetUserLevel(int reportsSubmitted)
        {
            return reportsSubmitted switch
            {
                0 => "Community Starter",
                1 => "Active Citizen",
                2 => "Engaged Resident",
                3 => "Community Champion",
                _ => "Community Hero"
            };
        }

        public EngagementResult UpdateUserProgress(int newReportCount)
        {
            var result = new EngagementResult { Success = true };

            result.NewLevel = GetUserLevel(newReportCount);
            result.NewAchievements = GetNewAchievementsForCount(newReportCount);
            result.AchievementUnlocked = result.NewAchievements.Any();

            return result;
        }

        private int CalculateCompletionPercentage(int reportsSubmitted)
        {
            return Math.Min(100, (reportsSubmitted * 33)); // Each report = 33% progress (3 reports = 99%)
        }

        private string GetNextLevel(int reportsSubmitted)
        {
            return reportsSubmitted switch
            {
                0 => "Active Citizen",
                1 => "Engaged Resident",
                2 => "Community Champion",
                3 => "Community Hero",
                _ => "Legend Status"
            };
        }

        private int GetPointsToNextLevel(int reportsSubmitted)
        {
            return reportsSubmitted switch
            {
                0 => 1,
                1 => 1,
                2 => 1,
                _ => 0
            };
        }

        private bool ShouldShowCelebration(int reportsSubmitted)
        {
            return reportsSubmitted == 3; // Show celebration when user reports 3 issues
        }

        private string GetCelebrationMessage(int reportsSubmitted, string userName)
        {
            if (reportsSubmitted == 3)
            {
                return $"CONGRATULATIONS {userName}!\n\nYou've just achieved COMMUNITY CHAMPION status! Your dedication to improving our community is truly inspiring. You've submitted 3 reports and are making a real difference in your neighborhood!\n\nKeep up the amazing work!";
            }
            return "";
        }

        private string CheckCategoryDiversityAchievement()
        {
            var userId = GetCurrentUserId();
            var allReports = _serviceRequestService.GetUserServiceRequests(userId);
            var categories = new CustomLinkedList<string>();

            foreach (var report in allReports)
            {
                // Skip the sample reports (ID 1 and 2)
                if (report.Id <= 2) continue;

                bool categoryExists = false;
                foreach (var existingCategory in categories)
                {
                    if (existingCategory == report.Category)
                    {
                        categoryExists = true;
                        break;
                    }
                }

                if (!categoryExists)
                {
                    categories.Add(report.Category);
                }
            }

            if (categories.Count >= 2)
                return "Category Explorer - Reported Multiple Issue Types";

            return "";
        }

        private CustomLinkedList<string> GetNewAchievementsForCount(int reportCount)
        {
            var newAchievements = new CustomLinkedList<string>();

            if (reportCount == 1)
                newAchievements.Add("First Report - Community Contributor");
            else if (reportCount == 3)
                newAchievements.Add("Community Champion - Three Reports Strong!");
            else if (reportCount == 5)
                newAchievements.Add("Community Hero - Five Reports Milestone");

            return newAchievements;
        }
    }
}