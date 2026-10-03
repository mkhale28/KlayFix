using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;
    private readonly IExpenseRepository _expenseRepo;

    public SaleService(ISaleRepository saleRepo, IProductRepository productRepo, IExpenseRepository expenseRepo)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
        _expenseRepo = expenseRepo;
    }

    public async Task<List<SaleDto>> GetAllAsync()
    {
        var sales = await _saleRepo.GetAllAsync().ConfigureAwait(false);
        return sales.Select(MapToDto).ToList();
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepo.GetByIdAsync(id).ConfigureAwait(false);
        return sale is null ? null : MapToDto(sale);
    }

    public async Task<SaleDto> CreateAsync(CreateSaleDto dto)
    {
        var paymentMethod = Enum.Parse<PaymentMethod>(dto.PaymentMethod, true);
        var items = new List<SaleItem>();
        decimal totalAmount = 0;
        decimal totalProfit = 0;

        foreach (var itemDto in dto.Items)
        {
            var totalPrice = itemDto.UnitPrice * itemDto.Quantity;
            var profit = (itemDto.UnitPrice - itemDto.PurchasePrice) * itemDto.Quantity;
            totalAmount += totalPrice;
            totalProfit += profit;

            items.Add(new SaleItem
            {
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                ProductBrand = itemDto.ProductBrand,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = totalPrice,
                PurchasePrice = itemDto.PurchasePrice
            });

            var product = await _productRepo.GetByIdAsync(itemDto.ProductId).ConfigureAwait(false);
            if (product is not null)
            {
                product.Quantity -= itemDto.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
                await _productRepo.UpdateAsync(product).ConfigureAwait(false);
            }
        }

        var netAmount = totalAmount - dto.Discount;
        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var sale = new Sale
        {
            SaleDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Discount = dto.Discount,
            NetAmount = netAmount,
            Profit = totalProfit,
            PaymentMethod = paymentMethod,
            InvoiceNumber = invoiceNumber,
            Items = items
        };

        sale = await _saleRepo.AddAsync(sale).ConfigureAwait(false);
        return MapToDto(sale);
    }

    public async Task<List<SaleDto>> GetByDateAsync(DateTime date)
    {
        var sales = await _saleRepo.GetByDateAsync(date).ConfigureAwait(false);
        return sales.Select(MapToDto).ToList();
    }

    public async Task<SalesReportDto> GetDailyReportAsync(DateTime date)
    {
        var sales = await _saleRepo.GetByDateAsync(date).ConfigureAwait(false);
        return new SalesReportDto
        {
            Date = date,
            TotalSales = sales.Sum(s => s.NetAmount),
            TotalProfit = sales.Sum(s => s.Profit),
            InvoiceCount = sales.Count
        };
    }

    public async Task<List<MonthlyReportDto>> GetMonthlyReportAsync(int year)
    {
        var sales = await _saleRepo.GetByYearAsync(year).ConfigureAwait(false);
        var results = new List<MonthlyReportDto>();

        for (int m = 1; m <= 12; m++)
        {
            var monthSales = sales.Where(s => s.SaleDate.Month == m).ToList();
            var expenses = await _expenseRepo.GetByDateRangeAsync(
                new DateTime(year, m, 1),
                new DateTime(year, m, 1).AddMonths(1).AddDays(-1)).ConfigureAwait(false);

            results.Add(new MonthlyReportDto
            {
                Year = year,
                Month = m,
                MonthName = new DateTime(year, m, 1).ToString("MMMM"),
                TotalSales = monthSales.Sum(s => s.NetAmount),
                TotalProfit = monthSales.Sum(s => s.Profit),
                TotalExpenses = expenses.Sum(e => e.Amount),
                NetProfit = monthSales.Sum(s => s.Profit) - expenses.Sum(e => e.Amount),
                InvoiceCount = monthSales.Count
            });
        }

        return results;
    }

    private static SaleDto MapToDto(Sale s) => new()
    {
        Id = s.Id,
        SaleDate = s.SaleDate,
        TotalAmount = s.TotalAmount,
        Discount = s.Discount,
        NetAmount = s.NetAmount,
        Profit = s.Profit,
        PaymentMethod = s.PaymentMethod.ToString(),
        InvoiceNumber = s.InvoiceNumber,
        Items = s.Items.Select(i => new SaleItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductBrand = i.ProductBrand,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }).ToList()
    };
}
