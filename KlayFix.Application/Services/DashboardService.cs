using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Enums;

namespace KlayFix.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ISaleRepository _saleRepo;
    private readonly IRepairRepository _repairRepo;
    private readonly IProductRepository _productRepo;
    private readonly IExpenseRepository _expenseRepo;

    public DashboardService(
        ISaleRepository saleRepo,
        IRepairRepository repairRepo,
        IProductRepository productRepo,
        IExpenseRepository expenseRepo)
    {
        _saleRepo = saleRepo;
        _repairRepo = repairRepo;
        _productRepo = productRepo;
        _expenseRepo = expenseRepo;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var todaySales = await _saleRepo.GetByDateAsync(today).ConfigureAwait(false);
        var lowStockProducts = await _productRepo.GetLowStockAsync(5).ConfigureAwait(false);
        var todayExpenses = await _expenseRepo.GetTotalByDateAsync(today).ConfigureAwait(false);

        var repairs = await _repairRepo.GetAllAsync().ConfigureAwait(false);

        return new DashboardDto
        {
            TotalSalesToday = todaySales.Sum(s => s.NetAmount),
            ProfitToday = todaySales.Sum(s => s.Profit),
            InvoiceCountToday = todaySales.Count,
            RepairsReceived = repairs.Count(r => r.Status == RepairStatus.Received),
            RepairsInProgress = repairs.Count(r => r.Status == RepairStatus.InProgress),
            RepairsCompleted = repairs.Count(r => r.Status == RepairStatus.Completed),
            LowStockCount = lowStockProducts.Count,
            TotalExpensesToday = todayExpenses,
            LowStockProducts = lowStockProducts.Select(p => new LowStockProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Quantity = p.Quantity
            }).ToList()
        };
    }
}
