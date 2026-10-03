using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using KlayFix.Desktop.Services;
using KlayFix.Domain.Enums;

namespace KlayFix.Desktop.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public NotificationService? NotificationService { get; set; }
    public ConfirmationService? ConfirmationService { get; set; }

    public BaseViewModel() { }

    public BaseViewModel(NotificationService notificationService, ConfirmationService? confirmationService = null)
    {
        NotificationService = notificationService;
        ConfirmationService = confirmationService;
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    protected bool RequireAdmin()
    {
        var currentUser = CurrentUserService.Instance;
        if (currentUser.IsAdmin) return true;

        var msg = "Access Denied: You do not have permission to access this feature.";
        NotifyWarning(msg);
        return false;
    }

    protected void NotifySuccess(string message) =>
        NotificationService?.ShowSuccess(message);

    protected void NotifyError(string message)
    {
        ErrorMessage = message;
        NotificationService?.ShowError(message);
    }

    protected void NotifyInfo(string message) =>
        NotificationService?.ShowInfo(message);

    protected void NotifyWarning(string message) =>
        NotificationService?.ShowWarning(message);

    protected async Task<bool> ConfirmAsync(string title, string message)
    {
        if (ConfirmationService is null) return true;
        return await ConfirmationService.ShowAsync(title, message);
    }

    protected async Task RunAsync(Func<Task> action, string? errorContext = null)
    {
        if (IsLoading) return;
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            await action();
        }
        catch (Exception ex)
        {
            var msg = $"{errorContext}: {ex.Message}";
            NotifyError(msg);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
