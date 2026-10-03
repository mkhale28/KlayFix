using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;

namespace KlayFix.Desktop.ViewModels;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IDashboardService _dashboardService;

    public DashboardViewModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
        _ = RunAsync(LoadDataAsync, LanguageService.Instance["Error_LoadDashboardFailed"]);
    }

    [ObservableProperty]
    private decimal _totalSalesToday;

    [ObservableProperty]
    private decimal _profitToday;

    [ObservableProperty]
    private int _invoiceCountToday;

    [ObservableProperty]
    private string _formattedInvoiceCount = string.Empty;

    [ObservableProperty]
    private int _repairsReceived;

    [ObservableProperty]
    private int _repairsInProgress;

    [ObservableProperty]
    private int _repairsCompleted;

    [ObservableProperty]
    private int _lowStockCount;

    [ObservableProperty]
    private decimal _totalExpensesToday;

    [ObservableProperty]
    private List<LowStockProductDto> _lowStockProducts = [];

    private async Task LoadDataAsync()
    {
        if (!RequireAdmin()) return;

        var data = await _dashboardService.GetDashboardAsync();
        TotalSalesToday = data.TotalSalesToday;
        ProfitToday = data.ProfitToday;
        InvoiceCountToday = data.InvoiceCountToday;
        FormattedInvoiceCount = string.Format(LanguageService.Instance["Dashboard_InvoiceCount"], InvoiceCountToday);
        RepairsReceived = data.RepairsReceived;
        RepairsInProgress = data.RepairsInProgress;
        RepairsCompleted = data.RepairsCompleted;
        LowStockCount = data.LowStockCount;
        TotalExpensesToday = data.TotalExpensesToday;
        LowStockProducts = data.LowStockProducts;
    }
}
