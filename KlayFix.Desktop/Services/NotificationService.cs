using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace KlayFix.Desktop.Services;

public enum NotificationType
{
    Success,
    Error,
    Info,
    Warning
}

public class NotificationService
{
    public ObservableCollection<NotificationItem> Notifications { get; } = [];

    public void Show(string message, NotificationType type = NotificationType.Info, int durationMs = 4000)
    {
        var notification = new NotificationItem
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
        };

        Notifications.Add(notification);
        StartAutoDismiss(notification, durationMs);
    }

    public void ShowSuccess(string message) => Show(message, NotificationType.Success);
    public void ShowError(string message) => Show(message, NotificationType.Error, 6000);
    public void ShowInfo(string message) => Show(message, NotificationType.Info);
    public void ShowWarning(string message) => Show(message, NotificationType.Warning, 5000);

    public void Dismiss(Guid id)
    {
        var item = Notifications.FirstOrDefault(n => n.Id == id);
        if (item is not null)
            Notifications.Remove(item);
    }

    public void Clear() => Notifications.Clear();

    public ICommand DismissCommand { get; }
    public NotificationService()
    {
        DismissCommand = new RelayCommand<Guid>(Dismiss);
    }

    private void StartAutoDismiss(NotificationItem notification, int delayMs)
    {
        var timer = new System.Windows.Threading.DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(delayMs);
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            Dismiss(notification.Id);
        };
        timer.Start();
    }
}

public class NotificationItem
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }

    public string IconKind => Type switch
    {
        NotificationType.Success => "CheckCircle",
        NotificationType.Error => "AlertCircle",
        NotificationType.Info => "Information",
        NotificationType.Warning => "Alert",
        _ => "Information"
    };

    public string AccentColor => Type switch
    {
        NotificationType.Success => "#10B981",
        NotificationType.Error => "#EF4444",
        NotificationType.Info => "#3B82F6",
        NotificationType.Warning => "#F59E0B",
        _ => "#3B82F6"
    };
}
