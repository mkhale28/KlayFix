using Moq;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Services;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Application;

public class SaleServiceTests
{
    private readonly Mock<ISaleRepository> _saleRepo;
    private readonly Mock<IProductRepository> _productRepo;
    private readonly Mock<IExpenseRepository> _expenseRepo;
    private readonly SaleService _service;

    public SaleServiceTests()
    {
        _saleRepo = new Mock<ISaleRepository>();
        _productRepo = new Mock<IProductRepository>();
        _expenseRepo = new Mock<IExpenseRepository>();
        _service = new SaleService(_saleRepo.Object, _productRepo.Object, _expenseRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSales()
    {
        var sales = new List<Sale>
        {
            new() { Id = 1, SaleDate = DateTime.UtcNow, TotalAmount = 500m, Discount = 0m, NetAmount = 500m, Profit = 200m, PaymentMethod = PaymentMethod.Cash, InvoiceNumber = "INV-001", Items = new List<SaleItem>() }
        };
        _saleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(sales);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("INV-001", result[0].InvoiceNumber);
        Assert.Equal("Cash", result[0].PaymentMethod);
        Assert.Equal(500m, result[0].NetAmount);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        _saleRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsSale()
    {
        var sale = new Sale
        {
            Id = 1, SaleDate = DateTime.UtcNow, TotalAmount = 1000m, Discount = 100m, NetAmount = 900m,
            Profit = 300m, PaymentMethod = PaymentMethod.Card, InvoiceNumber = "INV-001",
            Items = new List<SaleItem>
            {
                new() { Id = 1, ProductId = 1, ProductName = "Item", Quantity = 2, UnitPrice = 500m, TotalPrice = 1000m, PurchasePrice = 300m }
            }
        };
        _saleRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sale);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(900m, result!.NetAmount);
        Assert.Equal("Card", result.PaymentMethod);
        Assert.Single(result.Items);
        Assert.Equal("Item", result.Items[0].ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        _saleRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Sale?)null);
        var result = await _service.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithItems_CreatesSaleAndDecrementsStock()
    {
        var dto = new CreateSaleDto
        {
            Discount = 50m,
            PaymentMethod = "Cash",
            Items =
            [
                new CreateSaleItemDto { ProductId = 1, ProductName = "Item A", ProductBrand = "Brand A", Quantity = 2, UnitPrice = 500m, PurchasePrice = 300m },
                new CreateSaleItemDto { ProductId = 2, ProductName = "Item B", ProductBrand = "Brand B", Quantity = 1, UnitPrice = 200m, PurchasePrice = 100m }
            ]
        };

        var product1 = new Product { Id = 1, Name = "Item A", Quantity = 10 };
        var product2 = new Product { Id = 2, Name = "Item B", Quantity = 5 };
        _productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _productRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);

        Sale? captured = null;
        _saleRepo.Setup(r => r.AddAsync(It.IsAny<Sale>()))
            .Callback<Sale>(s => captured = s)
            .ReturnsAsync((Sale s) => s);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal(2, captured.Items.Count);
        Assert.Equal(1200m, captured.TotalAmount); // 2*500 + 1*200 = 1200
        Assert.Equal(50m, captured.Discount);
        Assert.Equal(1150m, captured.NetAmount); // 1200 - 50
        Assert.Equal(500m, captured.Profit); // (500-300)*2 + (200-100)*1 = 400 + 100 = 500
        Assert.Equal(PaymentMethod.Cash, captured.PaymentMethod);
        Assert.StartsWith("INV-", captured.InvoiceNumber);
        Assert.Equal(8, product1.Quantity); // 10 - 2
        Assert.Equal(4, product2.Quantity); // 5 - 1

        _productRepo.Verify(r => r.UpdateAsync(product1), Times.Once);
        _productRepo.Verify(r => r.UpdateAsync(product2), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NoItems_Throws()
    {
        var dto = new CreateSaleDto { Discount = 0, PaymentMethod = "Cash", Items = [] };

        await Assert.ThrowsAnyAsync<Exception>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WithCardPayment_SetsCardMethod()
    {
        var dto = new CreateSaleDto
        {
            Discount = 0, PaymentMethod = "Card",
            Items = [new CreateSaleItemDto { ProductId = 1, ProductName = "Test", Quantity = 1, UnitPrice = 100m, PurchasePrice = 50m }]
        };
        _productRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Test", Quantity = 10 });
        Sale? captured = null;
        _saleRepo.Setup(r => r.AddAsync(It.IsAny<Sale>()))
            .Callback<Sale>(s => captured = s)
            .ReturnsAsync((Sale s) => s);

        await _service.CreateAsync(dto);

        Assert.Equal(PaymentMethod.Card, captured!.PaymentMethod);
    }

    [Fact]
    public async Task GetByDateAsync_ReturnsDateFiltered()
    {
        var date = new DateTime(2025, 1, 15);
        var sales = new List<Sale>
        {
            new() { Id = 1, SaleDate = date, TotalAmount = 100m, Discount = 0, NetAmount = 100m, Profit = 30m, PaymentMethod = PaymentMethod.Cash, InvoiceNumber = "INV-001", Items = [] }
        };
        _saleRepo.Setup(r => r.GetByDateAsync(date)).ReturnsAsync(sales);

        var result = await _service.GetByDateAsync(date);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByDateAsync_NoSales_ReturnsEmpty()
    {
        _saleRepo.Setup(r => r.GetByDateAsync(It.IsAny<DateTime>())).ReturnsAsync([]);
        var result = await _service.GetByDateAsync(DateTime.UtcNow);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDailyReportAsync_CalculatesCorrectly()
    {
        var date = new DateTime(2025, 1, 15);
        var sales = new List<Sale>
        {
            new() { Id = 1, SaleDate = date, NetAmount = 500m, Profit = 200m },
            new() { Id = 2, SaleDate = date, NetAmount = 300m, Profit = 100m }
        };
        _saleRepo.Setup(r => r.GetByDateAsync(date)).ReturnsAsync(sales);

        var report = await _service.GetDailyReportAsync(date);

        Assert.Equal(date, report.Date);
        Assert.Equal(800m, report.TotalSales);
        Assert.Equal(300m, report.TotalProfit);
        Assert.Equal(2, report.InvoiceCount);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_Returns12Months()
    {
        var sales = new List<Sale>
        {
            new() { SaleDate = new DateTime(2025, 3, 15), NetAmount = 1000m, Profit = 400m }
        };
        _saleRepo.Setup(r => r.GetByYearAsync(2025)).ReturnsAsync(sales);
        _expenseRepo.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync([]);

        var reports = await _service.GetMonthlyReportAsync(2025);

        Assert.Equal(12, reports.Count);
        Assert.Equal("March", reports[2].MonthName);
        Assert.Equal(1000m, reports[2].TotalSales);
        Assert.Equal(400m, reports[2].TotalProfit);
        Assert.Equal(0m, reports[2].TotalExpenses);
        Assert.Equal(400m, reports[2].NetProfit);
        Assert.Equal(1, reports[2].InvoiceCount);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_AllMonthsEmpty()
    {
        _saleRepo.Setup(r => r.GetByYearAsync(2025)).ReturnsAsync([]);
        _expenseRepo.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync([]);

        var reports = await _service.GetMonthlyReportAsync(2025);

        Assert.All(reports, r => Assert.Equal(0, r.InvoiceCount));
        Assert.All(reports, r => Assert.Equal(0m, r.TotalSales));
    }
}
