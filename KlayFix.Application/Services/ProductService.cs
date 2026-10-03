using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;

namespace KlayFix.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync().ConfigureAwait(false);
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> SearchAsync(string query)
    {
        var products = await _repo.SearchAsync(query).ConfigureAwait(false);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Brand = dto.Brand,
            Category = dto.Category,
            IMEI = dto.IMEI,
            PurchasePrice = dto.PurchasePrice,
            SellingPrice = dto.SellingPrice,
            Quantity = dto.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        product = await _repo.AddAsync(product).ConfigureAwait(false);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(dto.Id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Product not found");

        product.Name = dto.Name;
        product.Brand = dto.Brand;
        product.Category = dto.Category;
        product.IMEI = dto.IMEI;
        product.PurchasePrice = dto.PurchasePrice;
        product.SellingPrice = dto.SellingPrice;
        product.Quantity = dto.Quantity;
        product.UpdatedAt = DateTime.UtcNow;
        product = await _repo.UpdateAsync(product).ConfigureAwait(false);
        return MapToDto(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Product not found");
        await _repo.DeleteAsync(product).ConfigureAwait(false);
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Brand = p.Brand,
        Category = p.Category.ToString(),
        IMEI = p.IMEI,
        PurchasePrice = p.PurchasePrice,
        SellingPrice = p.SellingPrice,
        Quantity = p.Quantity,
        CreatedAt = p.CreatedAt
    };
}
