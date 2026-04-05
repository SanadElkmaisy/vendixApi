﻿
using System.ComponentModel.DataAnnotations;

namespace VendixPos.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        public int GroupId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool? AuthenticationState { get; set; }

        [StringLength(50)]
        public string? AuthenticationCode { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public List<string> Permissions { get; set; } = new List<string>();
    }

    public class UserCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        public int GroupId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UserUpdateDto : UserCreateDto
    {
        public int UserId { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
