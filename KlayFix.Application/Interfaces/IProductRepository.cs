using KlayFix.Domain.Entities;

namespace KlayFix.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> SearchAsync(string query);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);
    Task<List<Product>> GetLowStockAsync(int threshold = 5);
}
