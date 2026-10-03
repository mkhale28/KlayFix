using KlayFix.Desktop.Services;
using KlayFix.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace KlayFix.Desktop.Views.Windows;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel, CurrentUserService currentUser)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.LoginSucceeded += () =>
        {
            DialogResult = true;
            Close();
        };

        Loaded += (_, _) =>
        {
            if (PasswordBox != null)
                PasswordBox.Focus();
        };

        PasswordBox.PasswordChanged += (_, _) =>
        {
            viewModel.Password = PasswordBox.Password;
        };
    }
}