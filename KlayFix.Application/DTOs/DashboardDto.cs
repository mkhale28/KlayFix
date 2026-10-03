namespace KlayFix.Application.DTOs;

public class DashboardDto
{
    public decimal TotalSalesToday { get; set; }
    public decimal ProfitToday { get; set; }
    public int InvoiceCountToday { get; set; }
    public int RepairsReceived { get; set; }
    public int RepairsInProgress { get; set; }
    public int RepairsCompleted { get; set; }
    public int LowStockCount { get; set; }
    public decimal TotalExpensesToday { get; set; }
    public List<LowStockProductDto> LowStockProducts { get; set; } = [];
}

public class LowStockProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class SalesReportDto
{
    public DateTime Date { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalProfit { get; set; }
    public int InvoiceCount { get; set; }
}

public class MonthlyReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int InvoiceCount { get; set; }
}
