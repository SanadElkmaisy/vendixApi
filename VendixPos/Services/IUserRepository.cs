
using VendixPos.DTOs;
using VendixPos.Models;

namespace VendixPos.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<User> AddUserAsync(UserCreateDto userDto);
        Task UpdateUserAsync(int id, UserUpdateDto userDto);
        Task DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(string username);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<IEnumerable<UserDto>> GetUsersByGroupAsync(int groupId);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<string>> GetUserPermissionsAsync(int userId);
        string GenerateJwtToken(UserDto user);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    }
}
