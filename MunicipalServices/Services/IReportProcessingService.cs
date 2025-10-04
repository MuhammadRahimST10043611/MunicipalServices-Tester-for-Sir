using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public interface IReportProcessingService
    {
        Task<ReportSubmissionResult> ProcessReportSubmissionAsync(ReportIssueViewModel model);
        EngagementResult ProcessUserEngagement(int newReportCount, int previousReportCount, string userName);
        ServiceResult ValidateReportSubmission(ReportIssueViewModel model);
        void SetCurrentUserId(int userId);
    }
}