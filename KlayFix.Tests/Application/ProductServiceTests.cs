using Moq;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Services;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Application;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Screen Protector", Brand = "Spigen", Category = ProductCategory.Accessory, SellingPrice = 150m, Quantity = 10 },
            new() { Id = 2, Name = "Phone Case", Brand = "OtterBox", Category = ProductCategory.Accessory, SellingPrice = 200m, Quantity = 5 }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Screen Protector", result[0].Name);
        Assert.Equal("Spigen", result[0].Brand);
        Assert.Equal("Accessory", result[0].Category);
        Assert.Equal(150m, result[0].SellingPrice);
    }

    [Fact]
    public async Task GetAllAsync_EmptyList_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithQuery_ReturnsFiltered()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "iPhone 15", Brand = "Apple", Category = ProductCategory.Phone, SellingPrice = 1000m, Quantity = 3 },
            new() { Id = 2, Name = "iPhone 15 Case", Brand = "Spigen", Category = ProductCategory.Accessory, SellingPrice = 200m, Quantity = 10 }
        };
        _mockRepo.Setup(r => r.SearchAsync("iPhone")).ReturnsAsync(products);

        var result = await _service.SearchAsync("iPhone");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SearchAsync_NoMatch_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.SearchAsync("xyz")).ReturnsAsync([]);

        var result = await _service.SearchAsync("xyz");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
    {
        var product = new Product { Id = 1, Name = "Test", Brand = "Brand", Category = ProductCategory.Phone, SellingPrice = 100m, Quantity = 5 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_CreatesProduct()
    {
        var dto = new CreateProductDto
        {
            Name = "New Product",
            Brand = "Brand",
            Category = ProductCategory.Phone,
            PurchasePrice = 50m,
            SellingPrice = 150m,
            Quantity = 10
        };

        Product? captured = null;
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => captured = p)
            .ReturnsAsync((Product p) => p);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("New Product", captured.Name);
        Assert.Equal(50m, captured.PurchasePrice);
        Assert.Equal(150m, captured.SellingPrice);
        Assert.Equal(10, captured.Quantity);
        Assert.NotEqual(default, captured.CreatedAt);
        Assert.NotEqual(default, captured.UpdatedAt);
        Assert.Equal(result.Name, dto.Name);
        Assert.Equal(result.SellingPrice, dto.SellingPrice);
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_UpdatesFields()
    {
        var existing = new Product { Id = 1, Name = "Old", Brand = "OldBrand", Category = ProductCategory.Phone, PurchasePrice = 50m, SellingPrice = 100m, Quantity = 5 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var dto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated",
            Brand = "NewBrand",
            Category = ProductCategory.Accessory,
            PurchasePrice = 60m,
            SellingPrice = 120m,
            Quantity = 8
        };

        Product? captured = null;
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Callback<Product>(p => captured = p)
            .ReturnsAsync((Product p) => p);

        var result = await _service.UpdateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("Updated", captured.Name);
        Assert.Equal("NewBrand", captured.Brand);
        Assert.Equal(ProductCategory.Accessory, captured.Category);
        Assert.Equal(60m, captured.PurchasePrice);
        Assert.Equal(120m, captured.SellingPrice);
        Assert.Equal(8, captured.Quantity);
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_Throws()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var dto = new UpdateProductDto { Id = 99 };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_Deletes()
    {
        var product = new Product { Id = 1, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockRepo.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _mockRepo.Verify(r => r.DeleteAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_Throws()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(99));
    }

    [Fact]
    public async Task SearchAsync_EmptyQuery_ReturnsAll()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "A", Brand = "B", Category = ProductCategory.Phone, SellingPrice = 100m, Quantity = 1 }
        };
        _mockRepo.Setup(r => r.SearchAsync("")).ReturnsAsync(products);

        var result = await _service.SearchAsync("");

        Assert.Single(result);
    }
}
