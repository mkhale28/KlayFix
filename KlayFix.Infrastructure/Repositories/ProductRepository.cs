using Microsoft.EntityFrameworkCore;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Infrastructure.Data;

namespace KlayFix.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<List<Product>> GetAllAsync()
    {
        return await _db.Products.OrderByDescending(p => p.UpdatedAt).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<Product>> SearchAsync(string query)
    {
        var q = query.ToLower();
        return await _db.Products
            .Where(p => p.Name.ToLower().Contains(q) ||
                        p.Brand.ToLower().Contains(q) ||
                        (p.IMEI != null && p.IMEI.Contains(q)))
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.FindAsync(id).ConfigureAwait(false);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync().ConfigureAwait(false);
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync().ConfigureAwait(false);
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<List<Product>> GetLowStockAsync(int threshold = 5)
    {
        return await _db.Products.Where(p => p.Quantity <= threshold).ToListAsync().ConfigureAwait(false);
    }
}
