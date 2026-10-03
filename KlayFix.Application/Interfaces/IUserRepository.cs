using KlayFix.Domain.Entities;

namespace KlayFix.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> AnyAsync();
    Task AddAsync(User user);
}