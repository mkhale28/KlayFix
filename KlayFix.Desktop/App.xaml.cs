using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using KlayFix.Desktop.Services;
using KlayFix.Desktop.ViewModels;
using KlayFix.Desktop.Views;
using KlayFix.Desktop.Views.Windows;
using KlayFix.Infrastructure;
using KlayFix.Infrastructure.Data;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Helpers;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace KlayFix.Desktop;

public partial class App : System.Windows.Application
{
    private static readonly string AppDataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "KlayFix");

    public static ServiceProvider ServiceProvider { get; private set; } = null!;
    public static AppLogger Logger { get; private set; } = null!;
    public static IConfiguration Configuration { get; private set; } = null!;
    public static string ConnectionString { get; private set; } = string.Empty;
    public static string BackupDirectory { get; private set; } = string.Empty;

    public App()
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        Directory.CreateDirectory(AppDataDir);

        DispatcherUnhandledException += (_, e) =>
        {
            Logger?.Fatal(e.Exception, "DispatcherUnhandledException");
            ShowFatalError("An unexpected error occurred. The application will close.");
            e.Handled = true;
            Current.Shutdown(-1);
        };

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Logger?.Fatal(e.ExceptionObject as Exception ?? new Exception("Unknown error"), "AppDomainUnhandledException");
            ShowFatalError("An unexpected error occurred. The application will close.");
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Logger?.Error(e.Exception, "UnobservedTaskException");
            e.SetObserved();
        };
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            // appsettings.json next to exe is now optional (installer may place it later)
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Logger = new AppLogger("KlayFix");
            Logger.LogStartup();

            // Read connection string if present
            ConnectionString = Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

            // If missing or invalid, prompt the user to configure connection
            if (string.IsNullOrWhiteSpace(ConnectionString) || !TestSqlConnection(ConnectionString))
            {
                Logger.Warn("No valid connection string found or cannot connect. Showing setup window.");

                var setup = new SetupWindow(ConnectionString);
                var result = setup.ShowDialog();
                if (result == true)
                {
                    // Reload configuration after setup
                    Configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();

                    ConnectionString = Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
                }
                else
                {
                    ShowFatalError("Application cannot start without a valid database connection. Please configure the connection and restart the application.");
                    Current.Shutdown(-1);
                    return;
                }
            }

            var backupDir = Configuration["AppSettings:BackupDirectory"];
            BackupDirectory = string.IsNullOrWhiteSpace(backupDir)
                ? Path.Combine(AppDataDir, "Backups")
                : backupDir;
            Directory.CreateDirectory(BackupDirectory);

            Logger.Info("Configuration loaded. Backup directory: {Dir}", BackupDirectory);

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            InitializeDatabase();
            SeedUsers();

            Logger.Info("Startup complete. Entering login loop.");
            ShowLoginLoop();
        }
        catch (Exception ex)
        {
            Logger?.Fatal(ex, "Application startup failed");
            ShowFatalError("Application failed to start. Please check logs for details.");
            Current.Shutdown(-1);
        }
    }

    private static bool TestSqlConnection(string conn)
    {
        try
        {
            // Add a short timeout if not present
            var builder = new SqlConnectionStringBuilder(conn);
            if (builder.ConnectTimeout == 0)
                builder.ConnectTimeout = 5;

            using var sql = new SqlConnection(builder.ConnectionString);
            sql.Open();
            sql.Close();
            return true;
        }
        catch (Exception ex)
        {
            Logger?.Warn($"SQL connection test failed: {ex.Message}");
            return false;

        }
    }

    private static void ShowLoginLoop()
    {
        try
        {
            while (true)
            {
                Logger.Debug("Showing login window...");
                var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
                var loginResult = loginWindow.ShowDialog();
                Logger.Debug("Login dialog result: {Result}", loginResult);

                if (loginResult != true)
                {
                    Logger.Info("Login cancelled. Shutting down.");
                    break;
                }

                Logger.Debug("Showing main window...");
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.ShowDialog();
                Logger.Debug("Main window closed.");

                var currentUser = ServiceProvider.GetRequiredService<CurrentUserService>();
                Logger.Debug("IsLoggedIn: {IsLoggedIn}", currentUser.IsLoggedIn);

                if (!currentUser.IsLoggedIn)
                {
                    Logger.Info("User logged out. Returning to login.");
                    continue;
                }

                Logger.Info("User remains logged in. Shutting down.");
                break;
            }

            Logger.Info("Application shutting down normally.");
            Logger.Dispose();
            Current.Shutdown();
        }
        catch (Exception ex)
        {
            Logger?.Fatal(ex, "Login loop error");
            try { MessageBox.Show($"Login loop error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
            Logger?.Dispose();
            Current.Shutdown(-1);
        }
    }

    private static void InitializeDatabase()
    {
        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Logger.Info("Testing database connection...");

        try
        {
            // Apply migrations (idempotent) instead of EnsureCreated
            db.Database.Migrate();
            Logger.Info("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to apply database migrations or connect to database.");
            throw new InvalidOperationException("Cannot connect to or initialize the database. Please verify the database server and connection string.");
        }
    }

    private static void SeedUsers()
    {
        using var scope = ServiceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        var seedOptions = new SeedUsersOptions
        {
            AdminUsername = Configuration["AppSettings:DefaultAdminUsername"] ?? "admin",
            AdminPassword = Configuration["AppSettings:DefaultAdminPassword"] ?? string.Empty,
            SalesUsername = Configuration["AppSettings:DefaultSalesUsername"] ?? "sales",
            SalesPassword = Configuration["AppSettings:DefaultSalesPassword"] ?? string.Empty
        };

        Logger.Info("Checking default users...");
        authService.SeedDefaultUsersAsync(seedOptions).GetAwaiter().GetResult();
        Logger.Info("Default users seeded successfully.");
    }

    private static void ShowFatalError(string message, Exception? ex = null)
    {
        try
        {
            // For clients show friendly message and point to logs
            MessageBox.Show(
                $"{message}\n\nPlease check the application logs for details: {Path.Combine(AppDataDir, "Logs")}",
                "KlayFix - Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch { }
    }

    internal static void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure(ConnectionString);

        services.AddSingleton<CurrentUserService>();
        services.AddSingleton<AuditService>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<ConfirmationService>();
        services.AddSingleton(s => new DatabaseBackupService(ConnectionString));

        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<SalesViewModel>();
        services.AddTransient<RepairsViewModel>();
        services.AddTransient<ExpensesViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<BackupViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
        services.AddTransient<Views.Pages.BackupPage>();
    }
}
