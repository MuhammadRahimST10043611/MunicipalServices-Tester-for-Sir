using MunicipalServices.Models;

namespace MunicipalServices.Services
{
    public interface IUserService
    {
        Task<ServiceResult> RegisterUserAsync(RegisterViewModel model);
        Task<UserLoginResult> LoginUserAsync(LoginViewModel model);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByNameAsync(string name);
        Task<bool> UserExistsAsync(string name);
        Task<ServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<CustomLinkedList<User>> GetAllUsersAsync();
    }

    public class UserLoginResult : ServiceResult
    {
        public User? User { get; set; }
        public bool IsAdmin { get; set; }
    }
}