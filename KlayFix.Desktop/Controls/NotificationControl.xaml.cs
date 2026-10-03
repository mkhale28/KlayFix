using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KlayFix.Desktop.Services;

namespace KlayFix.Desktop.Controls;

public partial class NotificationControl : UserControl
{
    private NotificationService? _service;

    public NotificationControl()
    {
        InitializeComponent();

        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _service = DataContext as NotificationService;
        if (_service is null) return;

        _service.Notifications.CollectionChanged += (_, args) =>
        {
            if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && args.NewItems?[0] is NotificationItem item)
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
                    AnimateIn(item));
            }
        };

        _service.Notifications.CollectionChanged += (_, args) =>
        {
            if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && args.OldItems?[0] is NotificationItem item)
            {
                var container = NotificationList.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                if (container is null) return;
                var border = FindChild<Border>(container);
                if (border is null) return;

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                fadeOut.Completed += (_, _) =>
                {
                    border.Visibility = Visibility.Collapsed;
                };
                border.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        };
    }

    private void AnimateIn(NotificationItem item)
    {
        var container = NotificationList.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
        if (container is null) return;

        var border = FindChild<Border>(container);
        if (border is null) return;

        if (!string.IsNullOrEmpty(item.AccentColor) &&
            ColorConverter.ConvertFromString(item.AccentColor) is Color color)
        {
            border.Background = new SolidColorBrush(color);
        }

        border.Opacity = 0;
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        var slideIn = new DoubleAnimation(80, 0, TimeSpan.FromMilliseconds(300));
        border.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        border.RenderTransform = new TranslateTransform();
        border.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
    }

    private static T? FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T t) return t;
            var result = FindChild<T>(child);
            if (result is not null) return result;
        }
        return null;
    }
}
