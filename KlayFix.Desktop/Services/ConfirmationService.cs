using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KlayFix.Desktop.Services;

public partial class ConfirmationService : ObservableObject
{
    private TaskCompletionSource<bool>? _tcs;

    [ObservableProperty]
    private bool _isVisible;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public ConfirmationService()
    {
        ConfirmCommand = new RelayCommand(Confirm);
        CancelCommand = new RelayCommand(Cancel);
    }

    public Task<bool> ShowAsync(string title, string message)
    {
        Title = title;
        Message = message;
        IsVisible = true;
        _tcs = new TaskCompletionSource<bool>();
        return _tcs.Task;
    }

    private void Confirm()
    {
        IsVisible = false;
        _tcs?.TrySetResult(true);
    }

    private void Cancel()
    {
        IsVisible = false;
        _tcs?.TrySetResult(false);
    }
}
