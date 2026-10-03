using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KlayFix.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CurrentUserService _currentUser;
    private readonly AuditService _audit;

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _currentUser = serviceProvider.GetRequiredService<CurrentUserService>();
        _audit = serviceProvider.GetRequiredService<AuditService>();
        NotificationService = _serviceProvider.GetRequiredService<NotificationService>();
        ConfirmationService = _serviceProvider.GetRequiredService<ConfirmationService>();

        SelectedViewModel = _currentUser.IsAdmin
            ? _serviceProvider.GetRequiredService<DashboardViewModel>()
            : _serviceProvider.GetRequiredService<SalesViewModel>();
    }

    public NotificationService NotificationService { get; }
    public ConfirmationService ConfirmationService { get; }

    public CurrentUserService CurrentUser => _currentUser;

    [ObservableProperty]
    private ObservableObject _selectedViewModel = null!;

    [ObservableProperty]
    private bool _isSidebarExpanded = true;

    public bool CanAccessDashboard => _currentUser.IsAdmin;
    public bool CanAccessProducts => _currentUser.IsAdmin;
    public bool CanAccessExpenses => _currentUser.IsAdmin;
    public bool CanAccessReports => _currentUser.IsAdmin;
    public bool CanAccessSettings => _currentUser.IsAdmin;
    public bool CanAccessUsers => _currentUser.IsAdmin;
    public bool CanAccessSales => _currentUser.IsAdmin || _currentUser.IsSalesman;
    public bool CanAccessRepairs => _currentUser.IsAdmin || _currentUser.IsSalesman;

    public event Action? LogoutRequested;

    private void NavigateTo<T>() where T : ObservableObject
    {
        var target = _serviceProvider.GetRequiredService<T>();

        if (target is BaseViewModel bvm)
        {
            if (bvm.NotificationService is null)
                bvm.NotificationService = NotificationService;
            if (bvm.ConfirmationService is null)
                bvm.ConfirmationService = ConfirmationService;
        }

        SelectedViewModel = target;
    }

    [RelayCommand]
    private void GoToBackup()
    {
        if (!CanAccessSettings)
        {
            NotificationService.ShowWarning("Access Denied");
            return;
        }
        NavigateTo<BackupViewModel>();
    }

    [RelayCommand]
    private void GoToDashboard()
    {
        if (!CanAccessDashboard)
        {
            NotificationService.ShowWarning("Access Denied");
            return;
        }
        NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void GoToProducts()
    {
        if (!CanAccessProducts)
        {
            NotificationService.ShowWarning("Access Denied");
            return;
        }
        NavigateTo<ProductsViewModel>();
    }

    [RelayCommand]
    private void GoToSales() => NavigateTo<SalesViewModel>();

    [RelayCommand]
    private void GoToRepairs() => NavigateTo<RepairsViewModel>();

    [RelayCommand]
    private void GoToExpenses()
    {
        if (!CanAccessExpenses)
        {
            NotificationService.ShowWarning("Access Denied");
            return;
        }
        NavigateTo<ExpensesViewModel>();
    }

    [RelayCommand]
    private void GoToReports()
    {
        if (!CanAccessReports)
        {
            NotificationService.ShowWarning("Access Denied");
            return;
        }
        NavigateTo<ReportsViewModel>();
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }

    [RelayCommand]
    private void Logout()
    {
        _audit.LogLogout(_currentUser.Username);
        _currentUser.Logout();
        LogoutRequested?.Invoke();
    }
}