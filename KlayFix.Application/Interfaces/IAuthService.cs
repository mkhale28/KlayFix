using KlayFix.Application.DTOs;

namespace KlayFix.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginDto dto);
    Task SeedDefaultUsersAsync(SeedUsersOptions? options = null);
}

public class SeedUsersOptions
{
    public string AdminUsername { get; set; } = "admin";
    public string AdminPassword { get; set; } = "Admin@123";
    public string SalesUsername { get; set; } = "sales";
    public string SalesPassword { get; set; } = "Sales@123";
}