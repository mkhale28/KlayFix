using KlayFix.Domain.Enums;

namespace KlayFix.Application.DTOs;

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public UserRole? Role { get; set; }
}