// Updated IFileUploadService.cs
using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public interface IFileUploadService
    {
        Task<CustomLinkedList<string>> SaveUploadedFilesAsync(CustomLinkedList<IFormFile> files);
        Task<CustomLinkedList<string>> SaveUploadedFilesAsync(CustomArray<IFormFile> files);
        ValidationResult ValidateFiles(CustomArray<IFormFile> files);
        FileValidationSettings GetValidationSettings();
    }
}