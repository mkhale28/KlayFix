using KlayFix.Application.DTOs;

namespace KlayFix.Application.Interfaces;

public interface IExpenseService
{
    Task<List<ExpenseDto>> GetAllAsync();
    Task<ExpenseDto?> GetByIdAsync(int id);
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto);
    Task DeleteAsync(int id);
    Task<List<ExpenseDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<decimal> GetTotalByDateAsync(DateTime date);
}
