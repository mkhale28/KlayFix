using KlayFix.Application.DTOs;
using KlayFix.Application.Helpers;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;

    public AuthService(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<LoginResult> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByUsernameAsync(dto.Username).ConfigureAwait(false);
        if (user is null)
            return new LoginResult { Success = false, Message = "Invalid username or password." };

        if (!user.IsActive)
            return new LoginResult { Success = false, Message = "Account is disabled." };

        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return new LoginResult { Success = false, Message = "Invalid username or password." };

        return new LoginResult
        {
            Success = true,
            Message = "Login successful.",
            UserId = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    public async Task SeedDefaultUsersAsync(SeedUsersOptions? options = null)
    {
        if (await _userRepo.AnyAsync().ConfigureAwait(false)) return;

        options ??= new SeedUsersOptions();

        var users = new List<User>
        {
            new()
            {
                Username = options.AdminUsername,
                PasswordHash = PasswordHasher.Hash(options.AdminPassword),
                FullName = "Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Username = options.SalesUsername,
                PasswordHash = PasswordHasher.Hash(options.SalesPassword),
                FullName = "Sales Person",
                Role = UserRole.Salesman,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var user in users)
            await _userRepo.AddAsync(user).ConfigureAwait(false);
    }
}