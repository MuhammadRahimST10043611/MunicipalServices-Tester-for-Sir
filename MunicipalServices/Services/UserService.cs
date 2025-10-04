using Microsoft.EntityFrameworkCore;
using MunicipalServices.Data;
using MunicipalServices.Models;
using System.Security.Cryptography;
using System.Text;

namespace MunicipalServices.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomHashTable<string, User> _userCache;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
            _userCache = new CustomHashTable<string, User>();
        }

        public async Task<ServiceResult> RegisterUserAsync(RegisterViewModel model)
        {
            var result = new ServiceResult();

            try
            {
                // Check if user already exists
                if (await UserExistsAsync(model.Name))
                {
                    result.Success = false;
                    result.Message = "A user with this name already exists.";
                    return result;
                }

                // Validate admin password attempt
                if (model.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    result.Success = false;
                    result.Message = "Cannot register with admin username.";
                    return result;
                }

                var user = new User
                {
                    Name = model.Name,
                    Password = HashPassword(model.Password),
                    IsAdmin = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Update cache
                _userCache.Add(user.Name, user);

                result.Success = true;
                result.Message = "User registered successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Registration failed: {ex.Message}";
            }

            return result;
        }

        public async Task<UserLoginResult> LoginUserAsync(LoginViewModel model)
        {
            var result = new UserLoginResult();

            try
            {
                // Special case for admin login with original password
                if (model.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == "Admin" && u.IsAdmin);

                    if (adminUser != null)
                    {
                        // Check both new hashed password and old simple password format
                        bool isValidPassword = VerifyPassword(model.Password, adminUser.Password) ||
                                             adminUser.Password == "AdminPass123!" ||
                                             VerifyOldPassword(model.Password, adminUser.Password);

                        if (isValidPassword && model.Password == "AdminPass123!")
                        {
                            result.Success = true;
                            result.User = adminUser;
                            result.IsAdmin = true;
                            result.Message = "Admin login successful.";
                            return result;
                        }
                    }
                }

                // Check cache first for regular users
                if (_userCache.TryGetValue(model.Name, out var cachedUser))
                {
                    if (VerifyPassword(model.Password, cachedUser.Password))
                    {
                        result.Success = true;
                        result.User = cachedUser;
                        result.IsAdmin = cachedUser.IsAdmin;
                        result.Message = "Login successful.";
                        return result;
                    }
                }

                // Check database for regular users
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Name == model.Name);

                if (user == null)
                {
                    result.Success = false;
                    result.Message = "Invalid username or password.";
                    return result;
                }

                if (!VerifyPassword(model.Password, user.Password))
                {
                    result.Success = false;
                    result.Message = "Invalid username or password.";
                    return result;
                }

                // Update cache
                if (!_userCache.ContainsKey(user.Name))
                {
                    _userCache.Add(user.Name, user);
                }

                result.Success = true;
                result.User = user;
                result.IsAdmin = user.IsAdmin;
                result.Message = "Login successful.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Login failed: {ex.Message}";
            }

            return result;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByNameAsync(string name)
        {
            // Check cache first
            if (_userCache.TryGetValue(name, out var cachedUser))
            {
                return cachedUser;
            }

            // Check database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == name);

            if (user != null && !_userCache.ContainsKey(user.Name))
            {
                _userCache.Add(user.Name, user);
            }

            return user;
        }

        public async Task<bool> UserExistsAsync(string name)
        {
            if (_userCache.ContainsKey(name))
            {
                return true;
            }

            return await _context.Users.AnyAsync(u => u.Name == name);
        }

        public async Task<ServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var result = new ServiceResult();

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    result.Success = false;
                    result.Message = "User not found.";
                    return result;
                }

                if (!VerifyPassword(currentPassword, user.Password))
                {
                    result.Success = false;
                    result.Message = "Current password is incorrect.";
                    return result;
                }

                user.Password = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                // Update cache
                if (_userCache.ContainsKey(user.Name))
                {
                    _userCache.Remove(user.Name);
                    _userCache.Add(user.Name, user);
                }

                result.Success = true;
                result.Message = "Password changed successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Password change failed: {ex.Message}";
            }

            return result;
        }

        public async Task<CustomLinkedList<User>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            var customList = new CustomLinkedList<User>();

            foreach (var user in users)
            {
                customList.Add(user);
            }

            return customList;
        }

        private string HashPassword(string password)
        {
            // Use SHA256 for better security
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "MUNICIPAL_SALT_2024"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }

        private bool VerifyOldPassword(string password, string hashedPassword)
        {
            // Check old base64 encoding method
            var oldHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + "SALT"));
            return oldHash == hashedPassword;
        }
    }
}