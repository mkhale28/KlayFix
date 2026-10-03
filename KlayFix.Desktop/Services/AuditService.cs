using System.IO;

namespace KlayFix.Desktop.Services;

public class AuditService
{
    private readonly string _logPath;

    public AuditService()
    {
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KlayFix");
        Directory.CreateDirectory(appData);
        _logPath = Path.Combine(appData, "audit.log");
    }

    public void Log(string action, string? username = null, string? detail = null)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var user = username ?? "SYSTEM";
            var entry = $"[{timestamp}] {user} | {action}";
            if (!string.IsNullOrEmpty(detail))
                entry += $" | {detail}";

            File.AppendAllText(_logPath, entry + Environment.NewLine);
        }
        catch
        {
            // Silently fail - audit logging should never crash the app
        }
    }

    public void LogLogin(string username, bool success)
    {
        Log(success ? "LOGIN_SUCCESS" : "LOGIN_FAILURE", username);
    }

    public void LogLogout(string username)
    {
        Log("LOGOUT", username);
    }

    public void LogUnauthorizedAccess(string username, string attemptedAction)
    {
        Log("UNAUTHORIZED_ACCESS", username, attemptedAction);
    }
}