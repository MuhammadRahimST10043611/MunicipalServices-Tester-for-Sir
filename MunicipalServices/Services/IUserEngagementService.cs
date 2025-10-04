// Updated IUserEngagementService.cs
using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public interface IUserEngagementService
    {
        UserEngagementStats GetUserEngagementStats();
        string GetMotivationalMessage(int reportsSubmitted, string userName);
        CustomLinkedList<string> GetAchievements(int reportsSubmitted);
        string GetUserLevel(int reportsSubmitted);
        UserNameResult SetUserName(string name);
        string GetUserName();
        bool ShouldShowNameModal();

        EngagementResult UpdateUserProgress(int newReportCount);
        ServiceResult ValidateUserName(string name);
    }
}