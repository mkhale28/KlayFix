using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Desktop.Services;
using KlayFix.Domain.Enums;
using System.ComponentModel;

namespace KlayFix.Desktop.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly CurrentUserService _currentUser;
    private readonly AuditService _audit;

    public LoginViewModel(
        IAuthService authService,
        CurrentUserService currentUser,
        AuditService audit)
    {
        _authService = authService;
        _currentUser = currentUser;
        _audit = audit;
    }

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public event Action? LoginSucceeded;
    public event Action? LoginFailed;

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Please enter username and password.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password.";
            return false;
        }
        return true;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        if (!Validate()) return;

        IsLoading = true;
        try
        {
            var result = await _authService.LoginAsync(new LoginDto
            {
                Username = Username,
                Password = Password
            });

            _audit.LogLogin(Username, result.Success);

            if (result.Success && result.UserId.HasValue && result.Role.HasValue)
            {
                _currentUser.Login(
                    result.UserId.Value,
                    result.Username!,
                    result.FullName!,
                    result.Role.Value);
                LoginSucceeded?.Invoke();
            }
            else
            {
                ErrorMessage = result.Message;
                LoginFailed?.Invoke();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login error: {ex.Message}";
            _audit.Log("LOGIN_ERROR", Username, ex.Message);
            LoginFailed?.Invoke();
        }
        finally
        {
            IsLoading = false;
        }
    }
}