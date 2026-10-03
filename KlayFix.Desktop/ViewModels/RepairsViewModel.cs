using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;

namespace KlayFix.Desktop.ViewModels;

public partial class RepairsViewModel : BaseViewModel
{
    private readonly IRepairService _repairService;

    public RepairsViewModel(IRepairService repairService)
    {
        _repairService = repairService;
        _ = RunAsync(LoadDataAsync, LanguageService.Instance["Error_LoadRepairsFailed"]);
    }

    [ObservableProperty]
    private ObservableCollection<RepairDto> _repairs = [];

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _statusFilter = "All";

    [ObservableProperty]
    private bool _isModalOpen;

    [ObservableProperty]
    private string _customerName = string.Empty;

    [ObservableProperty]
    private string _customerPhone = string.Empty;

    [ObservableProperty]
    private string _deviceModel = string.Empty;

    [ObservableProperty]
    private string? _deviceImei;

    [ObservableProperty]
    private string _issueDescription = string.Empty;

    [ObservableProperty]
    private decimal _cost;

    [ObservableProperty]
    private decimal _charge;

    [ObservableProperty]
    private string? _notes;

    public string[] StatusFilters => [
        LanguageService.Instance["Repairs_StatusAll"],
        LanguageService.Instance["Repairs_StatusReceived"],
        LanguageService.Instance["Repairs_StatusInProgress"],
        LanguageService.Instance["Repairs_StatusWaitingParts"],
        LanguageService.Instance["Repairs_StatusCompleted"],
        LanguageService.Instance["Repairs_StatusDelivered"]
    ];

    partial void OnSearchQueryChanged(string value) => _ = SearchDebouncedAsync(value);
    partial void OnStatusFilterChanged(string value) => _ = RunAsync(FilterAsync, LanguageService.Instance["Error_RepairFilterFailed"]);

    private async Task SearchDebouncedAsync(string query)
    {
        await Task.Delay(200);
        await FilterAsync();
    }

    private async Task LoadDataAsync()
    {
        var repairs = await _repairService.GetAllAsync();
        Repairs = new ObservableCollection<RepairDto>(repairs);
    }

    private async Task FilterAsync()
    {
        List<RepairDto> results;
        if (StatusFilter == "All")
            results = string.IsNullOrWhiteSpace(SearchQuery)
                ? await _repairService.GetAllAsync()
                : await _repairService.SearchAsync(SearchQuery);
        else
            results = await _repairService.GetByStatusAsync(StatusFilter);

        Repairs = new ObservableCollection<RepairDto>(results);
    }

    [RelayCommand]
    private void ClearSearch() => SearchQuery = string.Empty;

    [RelayCommand]
    private void OpenAddModal()
    {
        CustomerName = string.Empty;
        CustomerPhone = string.Empty;
        DeviceModel = string.Empty;
        DeviceImei = null;
        IssueDescription = string.Empty;
        Cost = 0;
        Charge = 0;
        Notes = null;
        ErrorMessage = string.Empty;
        IsModalOpen = true;
    }

    [RelayCommand]
    private void CloseModal() => IsModalOpen = false;

    [RelayCommand]
    private async Task SaveRepair()
    {
        if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(DeviceModel))
        {
            ErrorMessage = LanguageService.Instance["Error_CustomerDeviceRequired"];
            return;
        }

        await RunAsync(async () =>
        {
            await _repairService.CreateAsync(new CreateRepairDto
            {
                CustomerName = CustomerName, CustomerPhone = CustomerPhone,
                DeviceModel = DeviceModel, DeviceIMEI = DeviceImei,
                IssueDescription = IssueDescription, Cost = Cost, Charge = Charge, Notes = Notes
            });
            IsModalOpen = false;
            await LoadDataAsync();
            NotifySuccess(LanguageService.Instance["Notify_RepairCreated"]);
        }, LanguageService.Instance["Error_SaveRepairFailed"]);
    }

    [RelayCommand]
    private void PrintRepairReceipt(RepairDto repair)
    {
        if (repair is null) return;
        try
        {
            var dialog = new System.Windows.Controls.PrintDialog();
            if (dialog.ShowDialog() != true) return;
            var doc = new System.Windows.Documents.FlowDocument { PageWidth = 280, PageHeight = 500, FontFamily = new System.Windows.Media.FontFamily("Consolas"), FontSize = 11, TextAlignment = System.Windows.TextAlignment.Center };
            var receipt = new System.Windows.Documents.Section();
            var shopName = App.Configuration["ReceiptSettings:ShopName"] ?? "KlayFix Mobile Shop";
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(shopName)) { FontSize = 16, FontWeight = System.Windows.FontWeights.Bold, TextAlignment = System.Windows.TextAlignment.Center });
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("DEVICE RECEIPT")) { FontSize = 12, FontWeight = System.Windows.FontWeights.Bold, TextAlignment = System.Windows.TextAlignment.Center, Margin = new System.Windows.Thickness(0, 4, 0, 8) });
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40))) { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });
            var d = new System.Windows.Documents.Paragraph { TextAlignment = System.Windows.TextAlignment.Left };
            d.Inlines.Add(new System.Windows.Documents.Run($"Date: {repair.CreatedAt:MMM dd, yyyy  hh:mm tt}"));
            receipt.Blocks.Add(d);
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40))) { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });
            AddRepairLine(receipt, "Customer:", repair.CustomerName);
            AddRepairLine(receipt, "Phone:", repair.CustomerPhone);
            AddRepairLine(receipt, "Device:", repair.DeviceModel);
            if (!string.IsNullOrEmpty(repair.DeviceIMEI)) AddRepairLine(receipt, "IMEI:", repair.DeviceIMEI);
            AddRepairLine(receipt, "Issue:", repair.IssueDescription);
            AddRepairLine(receipt, "Status:", repair.Status);
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40))) { FontSize = 8, Margin = new System.Windows.Thickness(0, 4, 0, 4) });
            if (repair.Charge > 0)
            {
                var c = new System.Windows.Documents.Paragraph { TextAlignment = System.Windows.TextAlignment.Left, FontSize = 14, FontWeight = System.Windows.FontWeights.Bold };
                c.Inlines.Add(new System.Windows.Documents.Run("Service Charge:"));
                c.Inlines.Add(new System.Windows.Documents.Run($"  \u20B1{repair.Charge:N2}"));
                receipt.Blocks.Add(c);
            }
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(new string('\u2500', 40))) { FontSize = 8, Margin = new System.Windows.Thickness(0, 8, 0, 4) });
            receipt.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("Thank you for your business!")) { FontSize = 11, FontWeight = System.Windows.FontWeights.Bold, TextAlignment = System.Windows.TextAlignment.Center });
            doc.Blocks.Add(receipt);
            var paginator = ((System.Windows.Documents.IDocumentPaginatorSource)doc).DocumentPaginator;
            dialog.PrintDocument(paginator, $"Repair Receipt - {repair.CustomerName}");
        }
        catch (Exception ex) { NotifyError($"Print failed: {ex.Message}"); }
    }

    private static void AddRepairLine(System.Windows.Documents.Section section, string label, string value)
    {
        var p = new System.Windows.Documents.Paragraph { TextAlignment = System.Windows.TextAlignment.Left, Margin = new System.Windows.Thickness(0, 1, 0, 1) };
        p.Inlines.Add(new System.Windows.Documents.Run($"{label} ") { FontWeight = System.Windows.FontWeights.SemiBold });
        p.Inlines.Add(new System.Windows.Documents.Run(value));
        section.Blocks.Add(p);
    }

    [RelayCommand]
    private async Task UpdateStatus(RepairDto repair)
    {
        if (repair is null) return;
        await RunAsync(async () =>
        {
            var statuses = new[] { "Received", "InProgress", "WaitingParts", "Completed", "Delivered" };
            var currentIndex = Array.IndexOf(statuses, repair.Status);
            var nextStatus = currentIndex < statuses.Length - 1 ? statuses[currentIndex + 1] : statuses[currentIndex];

            await _repairService.UpdateStatusAsync(new UpdateRepairStatusDto { Id = repair.Id, Status = nextStatus });
            await LoadDataAsync();
            NotifySuccess($"{LanguageService.Instance["Notify_StatusUpdated"]} {nextStatus}");
        }, LanguageService.Instance["Error_StatusUpdateFailed"]);
    }
}
