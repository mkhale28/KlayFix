using Microsoft.EntityFrameworkCore;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Infrastructure.Data;

namespace KlayFix.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username).ConfigureAwait(false);
    }

    public async Task<bool> AnyAsync()
    {
        return await _db.Users.AnyAsync().ConfigureAwait(false);
    }

    public async Task AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }
}