using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace KlayFix.Desktop.Views.Windows;

public partial class SetupWindow : Window
{
    private readonly string _appsettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

    public SetupWindow(string initialConnection)
    {
        InitializeComponent();
        ConnText.Text = initialConnection;
    }

    private void BtnTest_Click(object sender, RoutedEventArgs e)
    {
        var conn = ConnText.Text.Trim();
        if (string.IsNullOrWhiteSpace(conn))
        {
            ShowStatus("Please enter a connection string.");
            return;
        }

        try
        {
            var builder = new SqlConnectionStringBuilder(conn);
            builder.ConnectTimeout = builder.ConnectTimeout == 0 ? 5 : builder.ConnectTimeout;
            using var sql = new SqlConnection(builder.ConnectionString);
            sql.Open();
            sql.Close();
            ShowStatus("Connection OK", success: true);
        }
        catch (Exception ex)
        {
            ShowStatus("Connection failed: " + ex.Message);
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        var conn = ConnText.Text.Trim();
        if (string.IsNullOrWhiteSpace(conn))
        {
            ShowStatus("Please enter a connection string.");
            return;
        }

        try
        {
            // write a minimal appsettings.json with connection string
            var doc = new
            {
                ConnectionStrings = new { DefaultConnection = conn },
                AppSettings = new { BackupDirectory = string.Empty }
            };
            var json = System.Text.Json.JsonSerializer.Serialize(doc, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_appsettingsPath, json);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            ShowStatus("Failed to save settings: " + ex.Message);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowStatus(string message, bool success = false)
    {
        StatusText.Text = message;
        StatusText.Foreground = success ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        StatusText.Visibility = Visibility.Visible;
    }
}
