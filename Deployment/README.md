# KlayFix ERP - Deployment Package

## Version 1.0.0

---

## Contents

```
Deployment/
├── KlayFixSetup_v1.0.0.exe      # Windows Installer (run this)
├── publish/                      # Standalone published app (manual install)
├── Database/
│   └── CreateDatabase.sql        # SQL script to manually create the database
├── Documentation/
│   ├── UserGuide.md              # End-user manual
│   ├── AdminGuide.md             # Administrator manual
│   └── ReleaseNotes.md           # Version information
├── Config/
│   └── appsettings.json          # Production configuration template
└── QA/
    └── QA_REPORT.md              # Quality assurance report
```

## Installation

### Option 1: Installer (Recommended)
Run `KlayFixSetup_v1.0.0.exe` and follow the wizard.

### Option 2: Manual
1. Copy the `publish/` folder to the target machine.
2. Ensure `.NET 9 Runtime` is installed.
3. Ensure `SQL Server` is installed and running.
4. Run `KlayFix.Desktop.exe`.

## Configuration

Edit `appsettings.json` to set the database connection string and shop details.

## First Launch

- Username: `admin`
- Password: `Admin@123`

Change the password immediately after first login.

## System Requirements

- Windows 10/11 64-bit
- SQL Server 2019+ or SQL Server Express
- .NET 9 Desktop Runtime
- 4 GB RAM, 500 MB disk space

---

*Copyright (c) 2026 KlayFix Solutions*
