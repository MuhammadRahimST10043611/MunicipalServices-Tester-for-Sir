using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public class ReportProcessingService : IReportProcessingService
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IUserEngagementService _userEngagementService;
        private int _currentUserId;

        public ReportProcessingService(
            IServiceRequestService serviceRequestService,
            IFileUploadService fileUploadService,
            IUserEngagementService userEngagementService)
        {
            _serviceRequestService = serviceRequestService;
            _fileUploadService = fileUploadService;
            _userEngagementService = userEngagementService;
        }

        public void SetCurrentUserId(int userId)
        {
            _currentUserId = userId;
        }

        private int GetCurrentUserId()
        {
            return _currentUserId;
        }

        public async Task<ReportSubmissionResult> ProcessReportSubmissionAsync(ReportIssueViewModel model)
        {
            var result = new ReportSubmissionResult();

            // Validate the submission
            var validationResult = ValidateReportSubmission(model);
            if (!validationResult.Success)
            {
                result.Success = false;
                result.Message = validationResult.Message;
                return result;
            }

            try
            {
                // Get current user's report count (not total system reports)
                var userId = GetCurrentUserId();
                var currentUserReports = _serviceRequestService.GetUserReportCount(userId);

                // Process file uploads with enhanced validation for multiple files
                var savedFiles = await ProcessFileUploadsAsync(model);

                // Create and save the service request
                var serviceRequest = CreateServiceRequest(model, savedFiles);
                serviceRequest.UserId = userId; // Set the user ID
                _serviceRequestService.AddServiceRequest(serviceRequest);

                // Process user engagement
                var newUserReports = currentUserReports + 1;
                var engagementResult = ProcessUserEngagement(newUserReports, currentUserReports, _userEngagementService.GetUserName());

                result.Success = true;
                result.RequestId = serviceRequest.Id;
                result.Message = $"Your issue has been reported successfully! Reference ID: {serviceRequest.Id}";

                // Add file upload summary to success message
                if (savedFiles.Count > 0)
                {
                    result.Message += $" ({savedFiles.Count} file{(savedFiles.Count != 1 ? "s" : "")} uploaded)";
                }

                // Checks for the celebration
                if (engagementResult.LevelChanged && newUserReports == 3)
                {
                    result.ShowCelebration = true;
                    result.CelebrationMessage = GenerateCelebrationMessage(_userEngagementService.GetUserName(), newUserReports);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "An error occurred while processing your report. Please try again.";

                Console.WriteLine($"Report processing error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return result;
            }
        }

        public EngagementResult ProcessUserEngagement(int newReportCount, int previousReportCount, string userName)
        {
            var result = new EngagementResult { Success = true };

            var previousLevel = GetUserLevelByReportCount(previousReportCount);
            var newLevel = GetUserLevelByReportCount(newReportCount);

            result.LevelChanged = previousLevel != newLevel;
            result.NewLevel = newLevel;
            result.PreviousLevel = previousLevel;

            // Check for new achievements
            var newAchievements = CheckForNewAchievements(newReportCount, previousReportCount);
            result.NewAchievements = newAchievements;
            result.AchievementUnlocked = newAchievements.Any();

            return result;
        }

        public ServiceResult ValidateReportSubmission(ReportIssueViewModel model)
        {
            var result = new ServiceResult();

            if (string.IsNullOrWhiteSpace(model.Location))
            {
                result.Success = false;
                result.Message = "Location is required.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(model.Category))
            {
                result.Success = false;
                result.Message = "Category is required.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                result.Success = false;
                result.Message = "Description is required.";
                return result;
            }

            if (model.Description.Length < 10)
            {
                result.Success = false;
                result.Message = "Description must be at least 10 characters long.";
                return result;
            }

            // Enhanced validation for multiple attached files using custom collections
            if (model.AttachedFiles != null && model.AttachedFiles.Any())
            {
                var fileValidation = ValidateAttachedFiles(model.AttachedFiles);
                if (!fileValidation.Success)
                {
                    result.Success = false;
                    result.Message = fileValidation.Message;
                    return result;
                }
            }

            result.Success = true;
            result.Message = "Validation passed.";
            return result;
        }

        // Enhanced the file upload to be able to process multiple files using custom collections
        private async Task<CustomLinkedList<string>> ProcessFileUploadsAsync(ReportIssueViewModel model)
        {
            try
            {
                // Handle multiple files from the model using custom collections
                if (model.AttachedFiles == null || !model.AttachedFiles.Any())
                {
                    return new CustomLinkedList<string>();
                }

                // Convert custom collection to custom array for processing
                var filesArray = new CustomArray<IFormFile>();
                foreach (var file in model.AttachedFiles)
                {
                    filesArray.Add(file);
                }

                // Validate files first
                var validationResult = _fileUploadService.ValidateFiles(filesArray);

                if (!validationResult.IsValid)
                {
                    Console.WriteLine($"File validation warnings: {string.Join("; ", validationResult.Messages)}");

                    // Filter out invalid files and continue with valid ones
                    var validFiles = new CustomArray<IFormFile>();
                    for (int i = 0; i < filesArray.Count; i++)
                    {
                        if (IsValidFile(filesArray[i]))
                        {
                            validFiles.Add(filesArray[i]);
                        }
                    }

                    if (validFiles.Count == 0)
                    {
                        Console.WriteLine("No valid files to process.");
                        return new CustomLinkedList<string>();
                    }

                    // Process only valid files
                    filesArray = validFiles;
                }

                // Use the existing SaveUploadedFilesAsync method that accepts custom arrays
                var savedFiles = await _fileUploadService.SaveUploadedFilesAsync(filesArray);

                Console.WriteLine($"Successfully processed {savedFiles.Count} files out of {model.AttachedFiles.Count} total files.");
                return savedFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File upload processing error: {ex.Message}");
                // Return empty list rather than crash the entire submission like it did before
                return new CustomLinkedList<string>();
            }
        }

        // Enhanced validation method for multiple files using custom collections
        private ServiceResult ValidateAttachedFiles(CustomCollection<IFormFile> files)
        {
            var result = new ServiceResult { Success = true };

            if (files == null || files.Count == 0)
                return result;

            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
            var errorMessages = new CustomLinkedList<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    errorMessages.Add($"Empty file detected: {file.FileName}");
                    continue;
                }

                if (file.Length > maxFileSize)
                {
                    errorMessages.Add($"File too large: {file.FileName} ({file.Length / 1024 / 1024:F2} MB, max 5MB)");
                    continue;
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    errorMessages.Add($"Unsupported file type: {file.FileName} ({extension})");
                }
            }

            if (errorMessages.Any())
            {
                result.Success = false;
                // Convert custom linked list to string for message
                var messageArray = new string[errorMessages.Count];
                errorMessages.CopyTo(messageArray, 0);
                result.Message = "File validation failed: " + string.Join("; ", messageArray);
            }

            return result;
        }

        // Helper method to validate individual files
        private bool IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size (5MB limit)
            const long maxSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSize)
                return false;

            // Check file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        private ServiceRequest CreateServiceRequest(ReportIssueViewModel model, CustomLinkedList<string> savedFiles)
        {
            return new ServiceRequest
            {
                Location = model.Location.Trim(),
                Category = model.Category,
                Description = model.Description.Trim(),
                AttachedFiles = savedFiles,
                DateReported = DateTime.Now,
                Status = "Submitted",
                Priority = DeterminePriorityByCategory(model.Category)
            };
        }

        private int DeterminePriorityByCategory(string category)
        {
            return category switch
            {
                "Water & Sewer" => 4,
                "Electricity" => 4,
                "Public Safety" => 5,
                "Roads & Transportation" => 3,
                "Waste Management" => 2,
                "Parks & Recreation" => 1,
                "Building & Planning" => 2,
                _ => 1
            };
        }

        private string GetUserLevelByReportCount(int reportCount)
        {
            return reportCount switch
            {
                0 => "Community Starter",
                1 => "Active Citizen",
                2 => "Engaged Resident",
                3 => "Community Champion",
                _ => "Community Hero"
            };
        }

        private CustomLinkedList<string> CheckForNewAchievements(int newCount, int previousCount)
        {
            var newAchievements = new CustomLinkedList<string>();

            // Used to check milestone achievements
            if (previousCount < 1 && newCount >= 1)
                newAchievements.Add("First Report - Community Contributor");

            if (previousCount < 3 && newCount >= 3)
                newAchievements.Add("Community Champion - Three Reports Strong!");

            if (previousCount < 5 && newCount >= 5)
                newAchievements.Add("Community Hero - Five Reports Milestone");

            // Used to check for multiple file uploads achievement
            if (newCount >= 1)
            {
                var userId = GetCurrentUserId();
                var userReports = _serviceRequestService.GetUserServiceRequests(userId);
                var multiFileReports = 0;

                foreach (var report in userReports)
                {
                    // Skip sample reports
                    if (report.Id <= 2) continue;

                    if (report.AttachedFiles.Count > 1)
                    {
                        multiFileReports++;
                    }
                }

                // Achievement for first multi-file report
                if (multiFileReports == 1 && previousCount == 0)
                {
                    newAchievements.Add("Tech Savvy - First Multi-File Report!");
                }
            }

            return newAchievements;
        }

        private string GenerateCelebrationMessage(string userName, int reportCount)
        {
            return reportCount switch
            {
                3 => $"CONGRATULATIONS {userName}!\n\nYou've just achieved COMMUNITY CHAMPION status! Your dedication to improving our community is truly inspiring. You've submitted 3 reports and are making a real difference in your neighborhood!\n\nKeep up the amazing work!",
                5 => $"AMAZING {userName}!\n\nYou've reached COMMUNITY HERO status! Your commitment to community improvement is exceptional!",
                _ => $"Great work {userName}! You're making a difference in your community!"
            };
        }
    }
}