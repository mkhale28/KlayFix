using KlayFix.Domain.Entities;

namespace KlayFix.Application.Interfaces;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync();
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale> AddAsync(Sale sale);
    Task<List<Sale>> GetByDateAsync(DateTime date);
    Task<List<Sale>> GetByMonthAsync(int year, int month);
    Task<List<Sale>> GetByYearAsync(int year);
}
