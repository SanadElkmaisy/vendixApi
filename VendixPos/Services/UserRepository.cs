using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VendixPos.Data;
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SelectItemBarNumSto> _logger;


        public UserRepository(AppDbContext context, IConfiguration configuration, ILogger<SelectItemBarNumSto> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Group)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    GroupId = u.GroupId,
                    GroupName = u.Group.GroupName,
                    IsActive = u.IsActive,
                    AuthenticationState = u.AuthenticationState,
                    AuthenticationCode = u.AuthenticationCode
                })
                .ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return null;

            var permissions = await GetUserPermissionsAsync(id);

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                GroupId = user.GroupId,
                GroupName = user.Group.GroupName,
                IsActive = user.IsActive,
                AuthenticationState = user.AuthenticationState,
                AuthenticationCode = user.AuthenticationCode,
                Permissions = permissions.ToList()
            };
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            var permissions = await GetUserPermissionsAsync(user.UserId);

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                GroupId = user.GroupId,
                GroupName = user.Group.GroupName,
                IsActive = user.IsActive,
                AuthenticationState = user.AuthenticationState,
                AuthenticationCode = user.AuthenticationCode,
                Permissions = permissions.ToList()
            };
        }

        public async Task<User> AddUserAsync(UserCreateDto userDto)
        {
            if (await UserExistsAsync(userDto.Username))
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                Username = userDto.Username,
                Password = HashPassword(userDto.Password),
                FullName = userDto.FullName,
                GroupId = userDto.GroupId,
                IsActive = userDto.IsActive,
                AuthenticationState = false,
                AuthenticationCode = GenerateAuthCode()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("User not found");

            // Check if username is being changed and if it already exists
            if (user.Username != userDto.Username && await UserExistsAsync(userDto.Username))
                throw new InvalidOperationException("Username already exists");

            user.Username = userDto.Username;
            user.FullName = userDto.FullName;
            user.GroupId = userDto.GroupId;
            user.IsActive = userDto.IsActive;

            // Only update password if provided
            if (!string.IsNullOrEmpty(userDto.Password) && userDto.Password != "********")
            {
                user.Password = HashPassword(userDto.Password);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            // Verify current password
            var currentPasswordHash = HashPassword(changePasswordDto.CurrentPassword);
            if (currentPasswordHash != user.Password)
            {
                _logger.LogWarning($"Password change failed for user {userId}: Current password incorrect");
                return false;
            }

            // Update to new password
            user.Password = HashPassword(changePasswordDto.NewPassword);

            // Optionally invalidate existing tokens if needed
            user.AuthenticationCode = GenerateAuthCode();

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password changed successfully for user {userId}");
            return true;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning($"User '{username}' not found or inactive");
                return false;
            }

            // Hash the input password
            var hashedInput = HashPassword(password);

            _logger.LogInformation($"Comparing hashes:");
            _logger.LogInformation($"  Input hash: {hashedInput}");
            _logger.LogInformation($"  Stored hash: {user.Password}");
            _logger.LogInformation($"  Match: {hashedInput == user.Password}");

            return hashedInput == user.Password;
        }
        [HttpGet("find-correct-encoding")]
        public IActionResult FindCorrectEncoding()
        {
            var password = "admin123";
            var expectedHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=";
            var results = new List<object>();

            // All possible encodings to test
            var encodingTests = new[]
            {
        new { Name = "UTF8", Encoding = Encoding.UTF8 },
        new { Name = "ASCII", Encoding = Encoding.ASCII },
        new { Name = "UTF32", Encoding = Encoding.UTF32 },
        new { Name = "Unicode (UTF16LE)", Encoding = Encoding.Unicode },
        new { Name = "BigEndianUnicode (UTF16BE)", Encoding = Encoding.BigEndianUnicode },
        new { Name = "Latin1", Encoding = Encoding.Latin1 },
        new { Name = "Default", Encoding = Encoding.Default }
    };

            using var sha256 = SHA256.Create();

            foreach (var test in encodingTests)
            {
                try
                {
                    var bytes = test.Encoding.GetBytes(password);
                    var hash = sha256.ComputeHash(bytes);
                    var hashString = Convert.ToBase64String(hash);

                    results.Add(new
                    {
                        Encoding = test.Name,
                        Hash = hashString,
                        Matches = hashString == expectedHash,
                        Bytes = BitConverter.ToString(bytes)
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        Encoding = test.Name,
                        Error = ex.Message
                    });
                }
            }

            // Also test with different password variations
            var passwordVariations = new[]
            {
        "admin123",
        "Admin123",
        "ADMIN123",
        "admin123!",
        "Admin@123",
        "1",
        "admin"
    };

            foreach (var pwd in passwordVariations)
            {
                var bytes = Encoding.UTF8.GetBytes(pwd);
                var hash = sha256.ComputeHash(bytes);
                var hashString = Convert.ToBase64String(hash);

                results.Add(new
                {
                    Test = $"Password: '{pwd}' (UTF8)",
                    Hash = hashString,
                    Matches = hashString == expectedHash
                });
            }

            return new OkObjectResult(results);
        }
        private string HashPassword(string password)
        {
            // Use MD5 instead of SHA256
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = md5.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public async Task<IEnumerable<UserDto>> GetUsersByGroupAsync(int groupId)
        {
            return await _context.Users
                .Where(u => u.GroupId == groupId)
                .Include(u => u.Group)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    GroupId = u.GroupId,
                    GroupName = u.Group.GroupName,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            return await _context.Users
                .Where(u => u.Username.Contains(searchTerm) ||
                           u.FullName.Contains(searchTerm))
                .Include(u => u.Group)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    GroupId = u.GroupId,
                    GroupName = u.Group.GroupName,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)
                .Join(_context.Groups,
                    u => u.GroupId,
                    g => g.GroupId,
                    (u, g) => g)
                .Join(_context.GroupPermissions,
                    g => g.GroupId,
                    gp => gp.GroupId,
                    (g, gp) => gp)
                .Join(_context.UsersPermissions,
                    gp => gp.PermissionId,
                    p => p.PermissionId,
                    (gp, p) => p.PermissionName)
                .Distinct()
                .ToListAsync();
        }

        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.GivenName, user.FullName ?? ""),
                    new Claim(ClaimTypes.Role, user.GroupName ?? "User")
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }

        private string GenerateAuthCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}
