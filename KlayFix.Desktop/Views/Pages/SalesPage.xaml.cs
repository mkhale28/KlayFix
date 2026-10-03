using System.Windows.Controls;
using System.Windows.Input;
using KlayFix.Desktop.ViewModels;

namespace KlayFix.Desktop.Views.Pages;

public partial class SalesPage : UserControl
{
    public SalesPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        IsVisibleChanged += OnIsVisibleChanged;
    }

    private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        SubscribeToFocusRequest();
        FocusScannerInput();
    }

    private void OnIsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isVisible && isVisible)
        {
            SubscribeToFocusRequest();
            FocusScannerInput();
        }
    }

    private void SubscribeToFocusRequest()
    {
        if (DataContext is SalesViewModel vm)
        {
            vm.FocusScannerRequested -= OnFocusScannerRequested;
            vm.FocusScannerRequested += OnFocusScannerRequested;
        }
    }

    private void OnFocusScannerRequested()
    {
        Dispatcher.BeginInvoke(() => FocusScannerInput());
    }

    public void FocusScannerInput()
    {
        if (ScannerInputBox != null)
        {
            ScannerInputBox.Focus();
            Keyboard.Focus(ScannerInputBox);
        }
    }
}
