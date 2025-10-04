using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public FileUploadService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<CustomLinkedList<string>> SaveUploadedFilesAsync(CustomLinkedList<IFormFile> files)
        {
            var savedFiles = new CustomLinkedList<string>();

            if (files == null || files.Count == 0)
                return savedFiles;

            var uploadsPath = GetUploadsPath();
            EnsureDirectoryExists(uploadsPath);

            foreach (var file in files)
            {
                // Added server-side validation for the final safety check
                if (file.Length > 0 && IsValidFile(file))
                {
                    var result = await SaveSingleFileAsync(file, uploadsPath);
                    if (result.Success)
                    {
                        savedFiles.Add(result.FilePath);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid file rejected: {file.FileName} (Size: {file.Length} bytes)");
                }
            }

            return savedFiles;
        }

        // NEW: Accept custom array instead of regular array
        public async Task<CustomLinkedList<string>> SaveUploadedFilesAsync(CustomArray<IFormFile> files)
        {
            var customFiles = new CustomLinkedList<IFormFile>();
            for (int i = 0; i < files.Count; i++)
            {
                // Server-side validation before adding to custom list
                if (IsValidFile(files[i]))
                {
                    customFiles.Add(files[i]);
                }
                else
                {
                    Console.WriteLine($"Invalid file rejected in array method: {files[i].FileName} (Size: {files[i].Length} bytes)");
                }
            }
            return await SaveUploadedFilesAsync(customFiles);
        }

        // CHANGED: Use custom collections instead of built-in ones
        public ValidationResult ValidateFiles(CustomArray<IFormFile> files)
        {
            var result = new ValidationResult { IsValid = true, Messages = new CustomLinkedList<string>() };

            if (files == null || files.Count == 0)
                return result;

            var settings = GetValidationSettings();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (!IsValidFile(file))
                {
                    result.IsValid = false;

                    if (file.Length > settings.MaxFileSize)
                    {
                        result.Messages.Add($"{file.FileName}: File too large ({file.Length / 1024 / 1024:F2} MB, max {settings.MaxFileSize / 1024 / 1024} MB)");
                    }
                    else
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        result.Messages.Add($"{file.FileName}: Unsupported file type ({extension})");
                    }
                }
            }

            return result;
        }

        public FileValidationSettings GetValidationSettings()
        {
            var allowedExtensions = new CustomArray<string>();
            var configExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>()
                ?? new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };

            foreach (var ext in configExtensions)
            {
                allowedExtensions.Add(ext);
            }

            return new FileValidationSettings
            {
                MaxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize", 5 * 1024 * 1024),
                AllowedExtensions = allowedExtensions
            };
        }

        private bool IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check for the file size (5MB limit)
            const long maxSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSize)
                return false;

            // Check for the file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                return false;

            var allowedMimeTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png", "image/gif",
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!string.IsNullOrEmpty(file.ContentType) && !allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        private async Task<FileProcessingResult> SaveSingleFileAsync(IFormFile file, string uploadsPath)
        {
            var result = new FileProcessingResult();

            try
            {
                if (!IsValidFile(file))
                {
                    result.Success = false;
                    result.Message = "File validation failed.";
                    return result;
                }

                var fileName = GenerateUniqueFileName(file.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                var fullPath = Path.GetFullPath(filePath);
                var uploadsFullPath = Path.GetFullPath(uploadsPath);

                if (!fullPath.StartsWith(uploadsFullPath))
                {
                    result.Success = false;
                    result.Message = "Invalid file path detected.";
                    return result;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result.Success = true;
                result.FilePath = $"/uploads/{fileName}";
                result.Message = "File saved successfully.";
            }
            catch (UnauthorizedAccessException ex)
            {
                result.Success = false;
                result.Message = "Access denied while saving file.";
                Console.WriteLine($"File save error (Access): {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                result.Success = false;
                result.Message = "Upload directory not found.";
                Console.WriteLine($"File save error (Directory): {ex.Message}");
            }
            catch (IOException ex)
            {
                result.Success = false;
                result.Message = "IO error while saving file.";
                Console.WriteLine($"File save error (IO): {ex.Message}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to save file: {ex.Message}";
                Console.WriteLine($"File save error (General): {ex.Message}");
            }

            return result;
        }

        private string GetUploadsPath()
        {
            var uploadPath = _configuration.GetValue<string>("FileUpload:UploadPath", "wwwroot/uploads");
            return Path.Combine(_environment.WebRootPath, "uploads");
        }

        private void EnsureDirectoryExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating upload directory: {ex.Message}");
                throw new InvalidOperationException("Could not create upload directory.", ex);
            }
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

            var sanitizedName = SanitizeFileName(nameWithoutExtension);

            // Used to generate unique filename with exact timestamp
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var uniqueId = Guid.NewGuid().ToString("N")[..8]; // First 8 characters of GUID

            return $"{timestamp}_{uniqueId}_{sanitizedName}{extension}";
        }

        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "file";

            // Remove or replace invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            // Remove additional potentially problematic characters
            var problematicChars = new[] { ' ', '(', ')', '[', ']', '{', '}', '&', '%', '$', '#', '@', '!', '^', '~', '`' };
            foreach (var ch in problematicChars)
            {
                sanitized = sanitized.Replace(ch, '_');
            }

            // Remove consecutive underscores
            while (sanitized.Contains("__"))
            {
                sanitized = sanitized.Replace("__", "_");
            }

            // Trim underscores from start and end
            sanitized = sanitized.Trim('_');

            // Limit length
            if (sanitized.Length > 50)
            {
                sanitized = sanitized.Substring(0, 50).TrimEnd('_');
            }

            // Ensure we have at least something
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = "file";
            }

            return sanitized;
        }
    }

    // Result classes needed by FileUploadService
    public class FileProcessingResult : ServiceResult
    {
        public string FilePath { get; set; } = "";
    }
}