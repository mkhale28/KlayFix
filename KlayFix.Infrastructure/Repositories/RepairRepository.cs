using Microsoft.EntityFrameworkCore;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;
using KlayFix.Infrastructure.Data;

namespace KlayFix.Infrastructure.Repositories;

public class RepairRepository : IRepairRepository
{
    private readonly AppDbContext _db;

    public RepairRepository(AppDbContext db) => _db = db;

    public async Task<List<Repair>> GetAllAsync()
    {
        return await _db.Repairs.OrderByDescending(r => r.CreatedAt).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Repair?> GetByIdAsync(int id)
    {
        return await _db.Repairs.FindAsync(id).ConfigureAwait(false);
    }

    public async Task<Repair> AddAsync(Repair repair)
    {
        _db.Repairs.Add(repair);
        await _db.SaveChangesAsync().ConfigureAwait(false);
        return repair;
    }

    public async Task<Repair> UpdateAsync(Repair repair)
    {
        _db.Repairs.Update(repair);
        await _db.SaveChangesAsync().ConfigureAwait(false);
        return repair;
    }

    public async Task<List<Repair>> GetByStatusAsync(string status)
    {
        var parsed = Enum.Parse<RepairStatus>(status, true);
        return await _db.Repairs.Where(r => r.Status == parsed)
            .OrderByDescending(r => r.CreatedAt).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<Repair>> SearchAsync(string query)
    {
        var q = query.ToLower();
        return await _db.Repairs
            .Where(r => r.CustomerName.ToLower().Contains(q) ||
                        r.CustomerPhone.Contains(q) ||
                        r.DeviceModel.ToLower().Contains(q) ||
                        (r.DeviceIMEI != null && r.DeviceIMEI.Contains(q)))
            .OrderByDescending(r => r.CreatedAt).ToListAsync().ConfigureAwait(false);
    }
}
