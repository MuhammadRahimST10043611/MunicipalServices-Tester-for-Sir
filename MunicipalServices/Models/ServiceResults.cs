namespace MunicipalServices.Models
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
    }

    public class UserNameResult : ServiceResult
    {
        public string UserName { get; set; } = "";
    }

    public class ReportSubmissionResult : ServiceResult
    {
        public int RequestId { get; set; }
        public bool ShowCelebration { get; set; }
        public string CelebrationMessage { get; set; } = "";
    }

    public class FileProcessingResult : ServiceResult
    {
        public string FilePath { get; set; } = "";
        public CustomLinkedList<string> ProcessedFiles { get; set; } = new CustomLinkedList<string>();
        public int ProcessedCount { get; set; }
    }

    public class EngagementResult : ServiceResult
    {
        public bool LevelChanged { get; set; }
        public string NewLevel { get; set; } = "";
        public string PreviousLevel { get; set; } = "";
        public bool AchievementUnlocked { get; set; }
        public CustomLinkedList<string> NewAchievements { get; set; } = new CustomLinkedList<string>();
    }

    public class FileValidationSettings
    {
        public long MaxFileSize { get; set; }
        public CustomArray<string> AllowedExtensions { get; set; } = new CustomArray<string>();
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public CustomLinkedList<string> Messages { get; set; } = new CustomLinkedList<string>();
    }

    public class MultipleFileValidationResult
    {
        public bool IsValid { get; set; }
        public int TotalFiles { get; set; }
        public int TotalValidFiles { get; set; }
        public int TotalInvalidFiles { get; set; }
        public CustomLinkedList<IFormFile> ValidFiles { get; set; } = new CustomLinkedList<IFormFile>();
        public CustomLinkedList<FileValidationError> InvalidFiles { get; set; } = new CustomLinkedList<FileValidationError>();
        public CustomLinkedList<string> GlobalErrors { get; set; } = new CustomLinkedList<string>();
    }

    public class FileValidationError
    {
        public string FileName { get; set; } = "";
        public CustomLinkedList<string> Errors { get; set; } = new CustomLinkedList<string>();
    }
}