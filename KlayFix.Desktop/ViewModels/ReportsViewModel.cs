using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;

namespace KlayFix.Desktop.ViewModels;

public partial class ReportsViewModel : BaseViewModel
{
    private readonly ISaleService _saleService;
    private readonly IProductService _productService;
    private readonly IRepairService _repairService;
    private readonly IExpenseService _expenseService;

    public ReportsViewModel(ISaleService saleService, IProductService productService,
        IRepairService repairService, IExpenseService expenseService)
    {
        _saleService = saleService;
        _productService = productService;
        _repairService = repairService;
        _expenseService = expenseService;
        _ = RunAsync(LoadDataAsync, LanguageService.Instance["Error_LoadReportsFailed"]);
    }

    [ObservableProperty]
    private int _selectedYear = DateTime.Today.Year;

    [ObservableProperty]
    private DateTime _dailyReportDate = DateTime.Today;

    [ObservableProperty]
    private SalesReportDto? _dailyReport;

    [ObservableProperty]
    private ObservableCollection<MonthlyReportDto> _monthlyReports = [];

    [ObservableProperty]
    private ObservableCollection<ProductDto> _inventoryReport = [];

    [ObservableProperty]
    private ObservableCollection<RepairDto> _repairReport = [];

    [ObservableProperty]
    private int _selectedTab;

    public int[] Years => [DateTime.Today.Year - 2, DateTime.Today.Year - 1, DateTime.Today.Year, DateTime.Today.Year + 1];

    partial void OnSelectedYearChanged(int value) => _ = LoadMonthlyReportAsync();
    partial void OnDailyReportDateChanged(DateTime value) => _ = LoadDailyReportAsync();

    private async Task LoadDataAsync()
    {
        if (!RequireAdmin()) return;

        await Task.WhenAll(
            LoadDailyReportAsync(),
            LoadMonthlyReportAsync(),
            LoadInventoryReportAsync(),
            LoadRepairReportAsync());
    }

    private async Task LoadDailyReportAsync()
    {
        try { DailyReport = await _saleService.GetDailyReportAsync(DailyReportDate); }
        catch { }
    }

    private async Task LoadMonthlyReportAsync()
    {
        try
        {
            var reports = await _saleService.GetMonthlyReportAsync(SelectedYear);
            MonthlyReports = new ObservableCollection<MonthlyReportDto>(reports);
        }
        catch { }
    }

    private async Task LoadInventoryReportAsync()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            InventoryReport = new ObservableCollection<ProductDto>(products);
        }
        catch { }
    }

    private async Task LoadRepairReportAsync()
    {
        try
        {
            var repairs = await _repairService.GetAllAsync();
            RepairReport = new ObservableCollection<RepairDto>(repairs);
        }
        catch { }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await RunAsync(LoadDataAsync, LanguageService.Instance["Error_RefreshFailed"]);
    }
}
