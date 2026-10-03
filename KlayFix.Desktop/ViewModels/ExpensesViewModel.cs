using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;

namespace KlayFix.Desktop.ViewModels;

public partial class ExpensesViewModel : BaseViewModel
{
    private readonly IExpenseService _expenseService;

    public ExpensesViewModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
        _ = RunAsync(LoadDataAsync, LanguageService.Instance["Error_LoadExpensesFailed"]);
    }

    [ObservableProperty]
    private ObservableCollection<ExpenseDto> _expenses = [];

    [ObservableProperty]
    private bool _isModalOpen;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _category = "Other";

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private DateTime _filterFrom = DateTime.Today.AddMonths(-1);

    [ObservableProperty]
    private DateTime _filterTo = DateTime.Today;

    public string[] Categories => [
        LanguageService.Instance["Expenses_CatRent"],
        LanguageService.Instance["Expenses_CatUtilities"],
        LanguageService.Instance["Expenses_CatSupplies"],
        LanguageService.Instance["Expenses_CatMaintenance"],
        LanguageService.Instance["Expenses_CatSalary"],
        LanguageService.Instance["Expenses_CatMarketing"],
        LanguageService.Instance["Expenses_CatOther"]
    ];

    private async Task LoadDataAsync()
    {
        if (!RequireAdmin()) return;

        var expenses = await _expenseService.GetByDateRangeAsync(FilterFrom, FilterTo);
        Expenses = new ObservableCollection<ExpenseDto>(expenses);
    }

    [RelayCommand]
    private async Task Filter()
    {
        await RunAsync(LoadDataAsync, LanguageService.Instance["Error_FilterFailed"]);
    }

    [RelayCommand]
    private void OpenAddModal()
    {
        Description = string.Empty;
        Amount = 0;
        Category = "Other";
        Date = DateTime.Today;
        Notes = null;
        ErrorMessage = string.Empty;
        IsModalOpen = true;
    }

    [RelayCommand]
    private void CloseModal() => IsModalOpen = false;

    [RelayCommand]
    private async Task SaveExpense()
    {
        if (string.IsNullOrWhiteSpace(Description) || Amount <= 0)
        {
            ErrorMessage = LanguageService.Instance["Error_DescAmountRequired"];
            return;
        }

        await RunAsync(async () =>
        {
            await _expenseService.CreateAsync(new CreateExpenseDto
            {
                Description = Description, Amount = Amount, Category = Category, Date = Date, Notes = Notes
            });
            IsModalOpen = false;
            await LoadDataAsync();
            NotifySuccess(LanguageService.Instance["Notify_ExpenseAdded"]);
        }, LanguageService.Instance["Error_SaveExpenseFailed"]);
    }

    [RelayCommand]
    private async Task DeleteExpense(int id)
    {
        var ls = LanguageService.Instance;
        var confirmed = await ConfirmAsync(ls["Confirm_DeleteExpense_Title"], ls["Confirm_DeleteExpense_Message"]);
        if (!confirmed) return;

        await RunAsync(async () =>
        {
            await _expenseService.DeleteAsync(id);
            await LoadDataAsync();
            NotifySuccess(LanguageService.Instance["Notify_ExpenseDeleted"]);
        }, LanguageService.Instance["Error_DeleteExpenseFailed"]);
    }
}
