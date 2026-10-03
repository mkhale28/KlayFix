using Microsoft.EntityFrameworkCore;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Infrastructure.Data;

namespace KlayFix.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _db;

    public SaleRepository(AppDbContext db) => _db = db;

    public async Task<List<Sale>> GetAllAsync()
    {
        return await _db.Sales.Include(s => s.Items)
            .OrderByDescending(s => s.SaleDate).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _db.Sales.Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id).ConfigureAwait(false);
    }

    public async Task<Sale> AddAsync(Sale sale)
    {
        _db.Sales.Add(sale);
        await _db.SaveChangesAsync().ConfigureAwait(false);
        return sale;
    }

    public async Task<List<Sale>> GetByDateAsync(DateTime date)
    {
        return await _db.Sales.Include(s => s.Items)
            .Where(s => s.SaleDate.Date == date.Date)
            .OrderByDescending(s => s.SaleDate).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<Sale>> GetByMonthAsync(int year, int month)
    {
        return await _db.Sales.Include(s => s.Items)
            .Where(s => s.SaleDate.Year == year && s.SaleDate.Month == month)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<Sale>> GetByYearAsync(int year)
    {
        return await _db.Sales.Include(s => s.Items)
            .Where(s => s.SaleDate.Year == year)
            .ToListAsync().ConfigureAwait(false);
    }
}
