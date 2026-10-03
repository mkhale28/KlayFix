using KlayFix.Application.DTOs;

namespace KlayFix.Application.Interfaces;

public interface ISaleService
{
    Task<List<SaleDto>> GetAllAsync();
    Task<SaleDto?> GetByIdAsync(int id);
    Task<SaleDto> CreateAsync(CreateSaleDto dto);
    Task<List<SaleDto>> GetByDateAsync(DateTime date);
    Task<SalesReportDto> GetDailyReportAsync(DateTime date);
    Task<List<MonthlyReportDto>> GetMonthlyReportAsync(int year);
}
