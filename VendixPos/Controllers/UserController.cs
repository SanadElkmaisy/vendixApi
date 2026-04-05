using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using VendixPos.DTOs;
using VendixPos.Services;

namespace VendixPos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> AddUserAsync([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newUser = await _userRepository.AddUserAsync(userDto);
                var userWithDetails = await _userRepository.GetUserByIdAsync(newUser.UserId);
                return CreatedAtAction(nameof(GetUserByIdAsync), new { id = newUser.UserId }, userWithDetails);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user");
                return StatusCode(500, "An internal server error occurred");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != userDto.UserId)
                return BadRequest("User ID mismatch");

            try
            {
                await _userRepository.UpdateUserAsync(id, userDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {userId}", id);
                return StatusCode(500, "An internal server error occurred");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            try
            {
                await _userRepository.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {userId}", id);
                return StatusCode(500, "An internal server error occurred");
            }
        }

        [HttpPost("{id}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Security check: User can only change their own password unless they have permission
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var currentUser = await _userRepository.GetUserByIdAsync(currentUserId);

                // Check if user is changing their own password or has admin permission
                bool isChangingOwnPassword = id == currentUserId;
                bool hasAdminPermission = currentUser.Permissions?.Contains("UserManagement") == true;

                if (!isChangingOwnPassword && !hasAdminPermission)
                    return Forbid("You can only change your own password");

                var success = await _userRepository.ChangePasswordAsync(id, changePasswordDto);

                if (!success)
                    return BadRequest("Current password is incorrect");

                return Ok(new { message = "Password changed successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {userId}", id);
                return StatusCode(500, "An internal server error occurred");
            }
        }

        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            // Get all claims for debugging
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            // Get the user ID from the token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var uniqueName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                message = "Authentication successful",
                userId = userId,
                name = name,
                uniqueName = uniqueName,
                claims = claims,
                isAuthenticated = User.Identity?.IsAuthenticated
            });
        }

        [HttpGet("diagnose-auth")]
        public async Task<IActionResult> DiagnoseAuth()
        {
            var result = new
            {
                // Check if authentication is set up
                AuthenticationExists = HttpContext.RequestServices.GetService<IAuthenticationService>() != null,

                // Check the authorization header
                AuthHeader = Request.Headers["Authorization"].ToString(),
                AuthHeaderExists = Request.Headers.ContainsKey("Authorization"),

                // Check user identity
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                AuthenticationType = User.Identity?.AuthenticationType,
                UserName = User.Identity?.Name,

                // Log all claims
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),

                // Check the endpoint's authorization requirements
                Endpoint = HttpContext.GetEndpoint()?.DisplayName,

                // Check if user is in any roles
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            // Log to console as well
            Console.WriteLine("=== DIAGNOSE AUTH ===");
            Console.WriteLine($"Auth Header: {Request.Headers["Authorization"]}");
            Console.WriteLine($"IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}:{c.Value}"))}");

            return Ok(result);
        }
        [HttpPost("debug-headers")]
        public IActionResult DebugHeaders([FromHeader] string authorization)
        {
            // Log all headers
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            return Ok(new
            {
                message = "Headers received",
                authorization = !string.IsNullOrEmpty(authorization) ? "Present" : "Missing",
                authValue = authorization?.Substring(0, Math.Min(30, authorization.Length)) + "...",
                allHeaders = headers
            });
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation($"Login attempt for username: {loginDto.Username}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValid = await _userRepository.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            _logger.LogInformation($"Password validation result: {isValid}");

            if (!isValid)
            {
                _logger.LogWarning($"Invalid credentials for user: {loginDto.Username}");
                return Unauthorized("Invalid username or password");
            }

            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                _logger.LogError($"User found during validation but not in GetUserByUsername: {loginDto.Username}");
                return Unauthorized("User not found");
            }

            var token = _userRepository.GenerateJwtToken(user);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.UserId);

            _logger.LogInformation($"Login successful for user: {user.Username}, UserId: {user.UserId}");

            return Ok(new AuthResponseDto
            {
                Token = token,
                User = user,
                Permissions = permissions.ToList()
            });
        }

        [HttpGet("test-password/{password}")]
        public IActionResult TestPasswordHash(string password)
        {
            var hash = HashPassword(password);
            var expectedHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=";

            return Ok(new
            {
                inputPassword = password,
                generatedHash = hash,
                expectedHash = expectedHash,
                match = hash == expectedHash,
                hashLength = hash.Length,
                expectedLength = expectedHash.Length,
                isBase64 = IsBase64String(hash)
            });
        }

        private bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByGroupAsync(int groupId)
        {
            var users = await _userRepository.GetUsersByGroupAsync(groupId);
            return Ok(users);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsersAsync([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest("Search term is required");

            var users = await _userRepository.SearchUsersAsync(term);
            return Ok(users);
        }

        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserPermissionsAsync(int id)
        {
            var permissions = await _userRepository.GetUserPermissionsAsync(id);
            return Ok(permissions);
        }

        private string HashPassword(string password)
        {
            // Use MD5 instead of SHA256
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = md5.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
    }
}
