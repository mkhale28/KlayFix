using Microsoft.EntityFrameworkCore;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Infrastructure.Data;

namespace KlayFix.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _db;

    public ExpenseRepository(AppDbContext db) => _db = db;

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _db.Expenses.OrderByDescending(e => e.Date).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _db.Expenses.FindAsync(id).ConfigureAwait(false);
    }

    public async Task<Expense> AddAsync(Expense expense)
    {
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync().ConfigureAwait(false);
        return expense;
    }

    public async Task DeleteAsync(Expense expense)
    {
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<List<Expense>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _db.Expenses
            .Where(e => e.Date >= from && e.Date <= to)
            .OrderByDescending(e => e.Date).ToListAsync().ConfigureAwait(false);
    }

    public async Task<decimal> GetTotalByDateAsync(DateTime date)
    {
        return await _db.Expenses.Where(e => e.Date.Date == date.Date)
            .SumAsync(e => e.Amount).ConfigureAwait(false);
    }
}
