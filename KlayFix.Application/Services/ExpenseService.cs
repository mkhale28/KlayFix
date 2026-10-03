using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repo;

    public ExpenseService(IExpenseRepository repo) => _repo = repo;

    public async Task<List<ExpenseDto>> GetAllAsync()
    {
        var expenses = await _repo.GetAllAsync().ConfigureAwait(false);
        return expenses.Select(MapToDto).ToList();
    }

    public async Task<ExpenseDto?> GetByIdAsync(int id)
    {
        var expense = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        return expense is null ? null : MapToDto(expense);
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Category = Enum.Parse<ExpenseCategory>(dto.Category, true),
            Date = dto.Date,
            Notes = dto.Notes
        };
        expense = await _repo.AddAsync(expense).ConfigureAwait(false);
        return MapToDto(expense);
    }

    public async Task DeleteAsync(int id)
    {
        var expense = await _repo.GetByIdAsync(id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Expense not found");
        await _repo.DeleteAsync(expense).ConfigureAwait(false);
    }

    public async Task<List<ExpenseDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var expenses = await _repo.GetByDateRangeAsync(from, to).ConfigureAwait(false);
        return expenses.Select(MapToDto).ToList();
    }

    public async Task<decimal> GetTotalByDateAsync(DateTime date)
    {
        return await _repo.GetTotalByDateAsync(date).ConfigureAwait(false);
    }

    private static ExpenseDto MapToDto(Expense e) => new()
    {
        Id = e.Id,
        Description = e.Description,
        Amount = e.Amount,
        Category = e.Category.ToString(),
        Date = e.Date,
        Notes = e.Notes
    };
}
