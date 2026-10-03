using System.ComponentModel;
using System.Runtime.CompilerServices;
using KlayFix.Domain.Enums;

namespace KlayFix.Desktop.Services;

public class CurrentUserService : INotifyPropertyChanged
{
    private static CurrentUserService? _instance;
    public static CurrentUserService Instance =>
        _instance ??= new CurrentUserService();

    private int? _userId;
    private string _username = string.Empty;
    private string _fullName = string.Empty;
    private UserRole? _role;
    private bool _isLoggedIn;

    public CurrentUserService()
    {
        _instance = this;
    }

    public int? UserId => _userId;
    public string Username => _username;
    public string FullName => _fullName;
    public UserRole? Role => _role;
    public bool IsLoggedIn => _isLoggedIn;

    public string RoleName =>
        _role switch
        {
            UserRole.Admin => "Admin",
            UserRole.Salesman => "Salesman",
            _ => string.Empty
        };

    public bool IsAdmin => _role == UserRole.Admin;
    public bool IsSalesman => _role == UserRole.Salesman;

    public void Login(int userId, string username, string fullName, UserRole role)
    {
        _userId = userId;
        _username = username;
        _fullName = fullName;
        _role = role;
        _isLoggedIn = true;
        OnPropertyChanged(nameof(UserId));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(FullName));
        OnPropertyChanged(nameof(Role));
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(RoleName));
        OnPropertyChanged(nameof(IsAdmin));
        OnPropertyChanged(nameof(IsSalesman));
    }

    public void Logout()
    {
        _userId = null;
        _username = string.Empty;
        _fullName = string.Empty;
        _role = null;
        _isLoggedIn = false;
        OnPropertyChanged(nameof(UserId));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(FullName));
        OnPropertyChanged(nameof(Role));
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(RoleName));
        OnPropertyChanged(nameof(IsAdmin));
        OnPropertyChanged(nameof(IsSalesman));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}