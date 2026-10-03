using Microsoft.Data.SqlClient;
using System.IO;

namespace KlayFix.Desktop.Services;

public class DatabaseBackupService
{
    private readonly string _connectionString;

    public DatabaseBackupService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string GetDatabaseName()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        return builder.InitialCatalog;
    }

    public async Task<string> BackupAsync(string backupFolder)
    {
        Directory.CreateDirectory(backupFolder);

        var dbName = GetDatabaseName();
        var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
        var fileName = $"Backup_{timestamp}.bak";
        var backupPath = Path.Combine(backupFolder, fileName);

        var sql = $@"
BACKUP DATABASE [{dbName}]
TO DISK = @backupPath
WITH FORMAT, MEDIANAME = 'KlayFixBackup', NAME = 'KlayFix Full Backup',
STATS = 10, COMPRESSION";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@backupPath", backupPath);

        await command.ExecuteNonQueryAsync();

        return backupPath;
    }

    public async Task<List<string>> GetBackupFilesAsync(string backupFolder)
    {
        if (!Directory.Exists(backupFolder))
            return [];

        var files = await Task.Run(() =>
            Directory.GetFiles(backupFolder, "Backup_*.bak")
                .OrderByDescending(f => f)
                .ToList());

        return files;
    }

    public async Task<List<BackupInfo>> GetBackupInfoAsync(string backupFolder)
    {
        if (!Directory.Exists(backupFolder))
            return [];

        var files = await Task.Run(() =>
        {
            var dir = new DirectoryInfo(backupFolder);
            return dir.GetFiles("Backup_*.bak")
                .OrderByDescending(f => f.LastWriteTime)
                .Select(f => new BackupInfo
                {
                    FileName = f.Name,
                    FilePath = f.FullName,
                    SizeBytes = f.Length,
                    CreatedAt = f.LastWriteTime,
                    SizeFormatted = FormatSize(f.Length)
                })
                .ToList();
        });

        return files;
    }

    public async Task<string> RestoreAsync(string backupFilePath)
    {
        var dbName = GetDatabaseName();
        var ts = DateTime.Now.ToString("yyyyMMddHHmmss");
        var restoredDbName = $"{dbName}_Restored_{ts}";

        var logicalFileNames = await GetLogicalFileNamesAsync(backupFilePath);

        var moveClauses = logicalFileNames.Count > 0
            ? string.Join(", ", logicalFileNames.Select(lf =>
                $"MOVE '{lf.LogicalName}' TO '{restoredDbName}_{lf.Type}.mdf'")) + ", "
            : "";

        var sql = $@"
RESTORE DATABASE [{restoredDbName}]
FROM DISK = @backupPath
WITH {moveClauses}REPLACE";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@backupPath", backupFilePath);

        await command.ExecuteNonQueryAsync();

        return restoredDbName;
    }

    private async Task<List<LogicalFileInfo>> GetLogicalFileNamesAsync(string backupFilePath)
    {
        var files = new List<LogicalFileInfo>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "RESTORE FILELISTONLY FROM DISK = @backupPath", connection);
            command.Parameters.AddWithValue("@backupPath", backupFilePath);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                files.Add(new LogicalFileInfo
                {
                    LogicalName = reader.GetString(0),
                    Type = reader.GetString(2)
                });
            }
        }
        catch
        {
            // Fallback: use defaults
            var dbName = GetDatabaseName();
            files.Add(new LogicalFileInfo { LogicalName = dbName, Type = "D" });
            files.Add(new LogicalFileInfo { LogicalName = $"{dbName}_log", Type = "L" });
        }

        return files;
    }

    private class LogicalFileInfo
    {
        public string LogicalName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

public class BackupInfo
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SizeFormatted { get; set; } = string.Empty;
}
