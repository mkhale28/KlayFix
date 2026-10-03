using Moq;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Services;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Application;

public class DashboardServiceTests
{
    private readonly Mock<ISaleRepository> _saleRepo;
    private readonly Mock<IRepairRepository> _repairRepo;
    private readonly Mock<IProductRepository> _productRepo;
    private readonly Mock<IExpenseRepository> _expenseRepo;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _saleRepo = new Mock<ISaleRepository>();
        _repairRepo = new Mock<IRepairRepository>();
        _productRepo = new Mock<IProductRepository>();
        _expenseRepo = new Mock<IExpenseRepository>();
        _service = new DashboardService(_saleRepo.Object, _repairRepo.Object, _productRepo.Object, _expenseRepo.Object);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsAggregatedData()
    {
        var today = DateTime.UtcNow.Date;
        _saleRepo.Setup(r => r.GetByDateAsync(today)).ReturnsAsync(new List<Sale>
        {
            new() { NetAmount = 1000m, Profit = 300m },
            new() { NetAmount = 500m, Profit = 150m }
        });
        _repairRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Repair>
        {
            new() { Status = RepairStatus.Received },
            new() { Status = RepairStatus.InProgress },
            new() { Status = RepairStatus.Completed },
            new() { Status = RepairStatus.InProgress }
        });
        _productRepo.Setup(r => r.GetLowStockAsync(5)).ReturnsAsync(new List<Product>
        {
            new() { Id = 1, Name = "Case", Brand = "Spigen", Quantity = 2 },
            new() { Id = 2, Name = "Tempered Glass", Brand = "Nillkin", Quantity = 1 }
        });
        _expenseRepo.Setup(r => r.GetTotalByDateAsync(today)).ReturnsAsync(200m);

        var result = await _service.GetDashboardAsync();

        Assert.Equal(1500m, result.TotalSalesToday); // 1000 + 500
        Assert.Equal(450m, result.ProfitToday); // 300 + 150
        Assert.Equal(2, result.InvoiceCountToday);
        Assert.Equal(1, result.RepairsReceived);
        Assert.Equal(2, result.RepairsInProgress);
        Assert.Equal(1, result.RepairsCompleted);
        Assert.Equal(2, result.LowStockCount);
        Assert.Equal(200m, result.TotalExpensesToday);
    }

    [Fact]
    public async Task GetDashboardAsync_EmptyDay_ReturnsZeros()
    {
        var today = DateTime.UtcNow.Date;
        _saleRepo.Setup(r => r.GetByDateAsync(today)).ReturnsAsync([]);
        _repairRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        _productRepo.Setup(r => r.GetLowStockAsync(5)).ReturnsAsync([]);
        _expenseRepo.Setup(r => r.GetTotalByDateAsync(today)).ReturnsAsync(0m);

        var result = await _service.GetDashboardAsync();

        Assert.Equal(0m, result.TotalSalesToday);
        Assert.Equal(0m, result.ProfitToday);
        Assert.Equal(0, result.InvoiceCountToday);
        Assert.Equal(0, result.RepairsReceived);
        Assert.Equal(0, result.RepairsInProgress);
        Assert.Equal(0, result.RepairsCompleted);
        Assert.Equal(0, result.LowStockCount);
        Assert.Equal(0m, result.TotalExpensesToday);
        Assert.Empty(result.LowStockProducts);
    }

    [Fact]
    public async Task GetDashboardAsync_LowStockProducts_AreMapped()
    {
        var today = DateTime.UtcNow.Date;
        _saleRepo.Setup(r => r.GetByDateAsync(today)).ReturnsAsync([]);
        _repairRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        _productRepo.Setup(r => r.GetLowStockAsync(5)).ReturnsAsync(new List<Product>
        {
            new() { Id = 10, Name = "Screen Protector", Brand = "Baseus", Quantity = 3 }
        });
        _expenseRepo.Setup(r => r.GetTotalByDateAsync(today)).ReturnsAsync(0m);

        var result = await _service.GetDashboardAsync();

        var lowStock = Assert.Single(result.LowStockProducts);
        Assert.Equal(10, lowStock.Id);
        Assert.Equal("Screen Protector", lowStock.Name);
        Assert.Equal("Baseus", lowStock.Brand);
        Assert.Equal(3, lowStock.Quantity);
    }
}
