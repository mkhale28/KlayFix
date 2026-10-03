using KlayFix.Domain.Entities;

namespace KlayFix.Application.Interfaces;

public interface IExpenseRepository
{
    Task<List<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<Expense> AddAsync(Expense expense);
    Task DeleteAsync(Expense expense);
    Task<List<Expense>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<decimal> GetTotalByDateAsync(DateTime date);
}
