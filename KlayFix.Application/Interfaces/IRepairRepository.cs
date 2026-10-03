using KlayFix.Domain.Entities;

namespace KlayFix.Application.Interfaces;

public interface IRepairRepository
{
    Task<List<Repair>> GetAllAsync();
    Task<Repair?> GetByIdAsync(int id);
    Task<Repair> AddAsync(Repair repair);
    Task<Repair> UpdateAsync(Repair repair);
    Task<List<Repair>> GetByStatusAsync(string status);
    Task<List<Repair>> SearchAsync(string query);
}
