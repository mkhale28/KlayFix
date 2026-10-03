# KlayFix ERP - Administrator Guide

## Version 1.0.0

---

## Table of Contents

1. [Installation](#installation)
2. [Configuration](#configuration)
3. [Database Management](#database-management)
4. [Backup & Restore](#backup-restore)
5. [User Management](#user-management)
6. [Troubleshooting](#troubleshooting)
7. [Logs & Diagnostics](#logs-diagnostics)

---

## Installation

### Prerequisites

1. **Windows 10 or Windows 11** (64-bit)
2. **SQL Server 2019+** or **SQL Server Express**
   - Download: https://go.microsoft.com/fwlink/?linkid=866658
   - Install with default instance (SQLEXPRESS) or named instance
3. **.NET 9 Runtime**
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0

### Installation Steps

#### Option 1: Using the Installer (Recommended)

1. Run `KlayFixSetup.exe`.
2. Follow the installation wizard.
3. Launch KlayFix ERP from the Start Menu.

#### Option 2: Manual Installation

1. Copy the published application folder to the target machine.
2. Ensure the `appsettings.json` file is in the application directory.
3. Create a shortcut to `KlayFix.Desktop.exe`.

### Verifying Installation

1. Launch the application.
2. The login screen should appear.
3. Log in with the default admin credentials:
   - Username: `admin`
   - Password: `Admin@123`

---

## Configuration

### Connection String

The database connection string is stored in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=KlayFixDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30;"
  }
}
```

#### Common Connection String Formats

| Scenario | Connection String |
|----------|------------------|
| SQL Server Express (local) | `Server=.\\SQLEXPRESS;Database=KlayFixDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server (default instance) | `Server=localhost;Database=KlayFixDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server (specific port) | `Server=localhost,1433;Database=KlayFixDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server with SQL Auth | `Server=.\\SQLEXPRESS;Database=KlayFixDB;User Id=sa;Password=your_password;TrustServerCertificate=True;` |

### App Settings

| Setting | Description | Default |
|---------|-------------|---------|
| `ApplicationName` | Display name for the application | KlayFix ERP |
| `Version` | Application version | 1.0.0 |
| `DefaultAdminUsername` | Admin username for seeding | admin |
| `DefaultAdminPassword` | Admin password for seeding | Admin@123 |
| `DefaultSalesUsername` | Sales username for seeding | sales |
| `DefaultSalesPassword` | Sales password for seeding | Sales@123 |
| `BackupDirectory` | Default backup folder (empty = AppData) | |
| `LogLevel` | Logging verbosity | Information |
| `EnableDiagnosticLogging` | Enable detailed diagnostic logs | false |

### Receipt Settings

| Setting | Description | Default |
|---------|-------------|---------|
| `ShopName` | Business name on receipts | KlayFix Mobile Shop |
| `Address` | Business address on receipts | 123 Main Street |
| `Phone` | Business phone on receipts | +1-555-0123 |
| `Footer` | Receipt footer text | Thank you for your business! |

---

## Database Management

### Initial Database Creation

The application automatically creates the database on first launch using Entity Framework's `EnsureCreated()` method. This creates all required tables and indexes.

### Manual Database Creation

To create the database manually:

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your SQL Server instance.
3. Open `CreateDatabase.sql` from the Deployment\Database folder.
4. Execute the script.

### Database Schema

The database contains the following tables:

| Table | Description |
|-------|-------------|
| `Users` | Application user accounts |
| `Products` | Product inventory |
| `Sales` | Sales transactions |
| `SaleItems` | Individual items within a sale |
| `Repairs` | Device repair tickets |
| `Expenses` | Business expense records |

---

## Backup & Restore

### Creating a Backup

**From the Application:**
1. Navigate to **Backup** in the sidebar.
2. Click **Create Backup**.
3. Select a destination folder.
4. The backup file will be created as `Backup_YYYY_MM_DD_HH_MM.bak`.

**From SQL Server Management Studio:**
1. Right-click on `KlayFixDB` database.
2. Select **Tasks > Backup**.
3. Choose backup destination.
4. Click **OK**.

**Using T-SQL:**
```sql
BACKUP DATABASE [KlayFixDB]
TO DISK = N'C:\Backups\KlayFixDB.bak'
WITH FORMAT, COMPRESSION;
```

### Restoring a Backup

**From the Application:**
1. Navigate to **Backup** in the sidebar.
2. Select a backup from the list.
3. Click **Restore Selected Backup**.
4. The restore creates a copy with a timestamp suffix.

**From SQL Server Management Studio:**
1. Right-click on **Databases**.
2. Select **Restore Database**.
3. Choose **Device** and select the .bak file.
4. Enter a new database name.
5. Click **OK**.

**Using T-SQL:**
```sql
RESTORE DATABASE [KlayFixDB_Restored]
FROM DISK = N'C:\Backups\KlayFixDB.bak'
WITH REPLACE;
```

### Backup Strategy Recommendations

| Frequency | Type | Retention |
|-----------|------|-----------|
| Daily | Full backup | 30 days |
| Weekly | Full backup | 3 months |
| Monthly | Full backup | 1 year |

---

## User Management

### Default Users

| Username | Password | Role | Description |
|----------|----------|------|-------------|
| admin | Admin@123 | Admin | Full system access |
| sales | Sales@123 | Salesman | Sales and Repairs only |

### Role Permissions

| Feature | Admin | Salesman |
|---------|-------|----------|
| Dashboard | ✓ | |
| Products (CRUD) | ✓ | |
| Point of Sale | ✓ | ✓ |
| Repairs | ✓ | ✓ |
| Expenses | ✓ | |
| Reports | ✓ | |
| Backup & Restore | ✓ | |

### Changing Passwords

Default passwords should be changed immediately after installation. Password changes are managed through the application's user settings (coming in a future version). For now, passwords can be updated directly in the database:

```sql
-- The password hash must be generated by the application
UPDATE [Users] SET [PasswordHash] = '<new_hash>' WHERE [Username] = 'admin';
```

---

## Troubleshooting

### Application Won't Start

**Symptom**: Application crashes immediately or shows a connection error.

**Solutions:**

1. **Check SQL Server is running:**
   - Open Services (`services.msc`)
   - Look for `SQL Server (SQLEXPRESS)` or `SQL Server (MSSQLSERVER)`
   - Ensure the service is running

2. **Verify connection string:**
   - Open `appsettings.json` in the application directory
   - Verify the `DefaultConnection` string is correct
   - For SQL Server Express: `Server=.\\SQLEXPRESS`
   - For SQL Server default: `Server=localhost`

3. **Check Windows Firewall:**
   - Ensure SQL Server port (default 1433) is open
   - Add an inbound rule for `sqlservr.exe`

4. **Review crash logs:**
   - Logs are stored at: `%LOCALAPPDATA%\KlayFix\Logs\`
   - Check for error details

### Database Connection Errors

**Symptom**: "Cannot connect to database" error on startup.

**Solutions:**

1. Enable TCP/IP for SQL Server:
   - Open **SQL Server Configuration Manager**
   - Navigate to **SQL Server Network Configuration > Protocols for SQLEXPRESS**
   - Enable **TCP/IP**
   - Restart SQL Server service

2. Verify SQL Server Browser service is running:
   - Open **Services** (`services.msc`)
   - Start **SQL Server Browser**
   - Set startup type to **Automatic**

3. Check the logs at `%LOCALAPPDATA%\KlayFix\Logs\klayfix-.log`

### Blank or Missing Data

**Symptom**: Pages show no data when data should exist.

**Solutions:**

1. **Refresh the page** by navigating away and back.
2. **Check database connectivity** using the logs.
3. **Verify data exists** in SQL Server Management Studio.

---

## Logs & Diagnostics

### Log Locations

| Log Type | Location |
|----------|----------|
| Application Logs | `%LOCALAPPDATA%\KlayFix\Logs\klayfix-.log` |
| Error Logs | `%LOCALAPPDATA%\KlayFix\Logs\errors-.log` |
| Audit Log | `%LOCALAPPDATA%\KlayFix\audit.log` |
| Crash Log | `%LOCALAPPDATA%\KlayFix\crash.log` |

### Log Retention

- Application logs: 30 days (rolling)
- Error logs: 90 days (rolling)
- Maximum file size: 10 MB per file

### Reading Logs

Logs use the following format:
```
2026-06-25 14:30:00.123 [INF] Application started
2026-06-25 14:30:05.456 [ERR] Database connection failed: Cannot open database
```

### Enabling Diagnostic Logging

To enable more detailed logs:

1. Open `appsettings.json`.
2. Set `"EnableDiagnosticLogging": true`.
3. Restart the application.

---

## Deployment Package Structure

```
Deployment/
├── KlayFixSetup_v1.0.0.exe     # Installer
├── Database/
│   ├── CreateDatabase.sql      # Database creation script
│   └── KlayFixDB.bak           # Empty database backup
├── Documentation/
│   ├── UserGuide.pdf           # User manual
│   ├── AdminGuide.pdf          # Administrator manual
│   └── ReleaseNotes.md         # Version release notes
├── Config/
│   └── appsettings.json        # Production configuration template
└── QA/
    └── QA_REPORT.md            # Quality assurance report
```

---

## Release Notes

### Version 1.0.0 (June 25, 2026)

**Initial Release Features:**
- User authentication with RBAC (Admin/Salesman)
- Product inventory management
- Point of Sale with barcode scanner support
- Repair ticket management with status tracking
- Expense tracking
- Business reports (daily, monthly, inventory, repairs)
- Receipt printing
- Bilingual interface (English/Arabic)
- Database backup & restore
- Production logging with Serilog

---

*Document version: 1.0.0*
*Last updated: June 25, 2026*
