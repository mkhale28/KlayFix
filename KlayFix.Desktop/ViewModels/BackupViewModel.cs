using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KlayFix.Desktop.Services;
using Microsoft.Win32;

namespace KlayFix.Desktop.ViewModels;

public partial class BackupViewModel : BaseViewModel
{
    private readonly DatabaseBackupService _backupService;

    public BackupViewModel(DatabaseBackupService backupService)
    {
        _backupService = backupService;
        _ = LoadBackupsAsync();
    }

    [ObservableProperty]
    private ObservableCollection<BackupInfo> _backups = [];

    [ObservableProperty]
    private BackupInfo? _selectedBackup;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _backupFolder = App.BackupDirectory;

    [ObservableProperty]
    private bool _isRestoring;

    private async Task LoadBackupsAsync()
    {
        try
        {
            var list = await _backupService.GetBackupInfoAsync(BackupFolder);
            Backups = new ObservableCollection<BackupInfo>(list);
        }
        catch (Exception ex)
        {
            NotifyError($"Failed to load backups: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task CreateBackup()
    {
        await RunAsync(async () =>
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Backup Location",
                DefaultDirectory = BackupFolder
            };

            if (folderDialog.ShowDialog() != true)
                return;

            var targetFolder = folderDialog.FolderName;
            var path = await _backupService.BackupAsync(targetFolder);
            BackupFolder = targetFolder;
            StatusMessage = $"Backup created: {System.IO.Path.GetFileName(path)}";
            NotifySuccess($"Database backup created successfully.\n{path}");
            await LoadBackupsAsync();
        }, "Backup failed");
    }

    [RelayCommand]
    private async Task RestoreBackup()
    {
        if (SelectedBackup is null)
        {
            NotifyWarning("Please select a backup file to restore.");
            return;
        }

        var confirmed = await ConfirmAsync(
            "Restore Backup",
            $"Are you sure you want to restore from:\n{SelectedBackup.FileName}?\n\nThis will create a restored copy of the database.");

        if (!confirmed) return;

        IsRestoring = true;
        try
        {
            var dbName = await _backupService.RestoreAsync(SelectedBackup.FilePath);
            StatusMessage = $"Restored as: {dbName}";
            NotifySuccess($"Database restored successfully as '{dbName}'. Please update connection string to use this database.");
        }
        catch (Exception ex)
        {
            NotifyError($"Restore failed: {ex.Message}");
        }
        finally
        {
            IsRestoring = false;
        }
    }

    [RelayCommand]
    private async Task OpenBackupFolder()
    {
        await Task.Run(() =>
        {
            if (System.IO.Directory.Exists(BackupFolder))
                System.Diagnostics.Process.Start("explorer.exe", BackupFolder);
        });
    }

    [RelayCommand]
    private async Task DeleteBackup(BackupInfo backup)
    {
        if (backup is null) return;

        var confirmed = await ConfirmAsync(
            "Delete Backup",
            $"Delete backup file?\n{backup.FileName} ({backup.SizeFormatted})");

        if (!confirmed) return;

        try
        {
            if (System.IO.File.Exists(backup.FilePath))
                System.IO.File.Delete(backup.FilePath);

            Backups.Remove(backup);
            NotifySuccess("Backup deleted.");
        }
        catch (Exception ex)
        {
            NotifyError($"Failed to delete backup: {ex.Message}");
        }
    }
}
