# KlayFix ERP - Release Notes

## Version 1.0.0

**Release Date:** June 25, 2026

---

### Overview

KlayFix ERP v1.0.0 is the initial production release of the desktop ERP system for mobile phone repair shops. Built on .NET 9 with WPF and Clean Architecture, it provides a complete solution for inventory management, point-of-sale operations, repair tracking, and business reporting.

### New Features

#### Core System
- User authentication with password hashing (PBKDF2/SHA-256)
- Role-based access control (Admin, Salesman)
- Bilingual interface (English/Arabic) with runtime switching
- Production logging with Serilog (file rotation, 30-day retention)

#### Products Management
- Full CRUD operations for product inventory
- Search by name, brand, or IMEI
- Category classification (Phone, Accessory, Tablet, Part, Other)
- Low stock alerts

#### Point of Sale
- Product search and grid browsing
- Shopping cart with quantity management
- Barcode/IMEI scanner support
- Discount application
- Cash/Card payment methods
- Receipt printing (FlowDocument + PrintDialog)

#### Repairs Management
- Repair ticket creation and tracking
- Status workflow: Received → In Progress → Waiting Parts → Completed → Delivered
- Search by customer, device, or IMEI
- Status filtering

#### Expenses
- Expense tracking with categories
- Date-range filtering
- Full CRUD operations

#### Reports
- Daily sales report
- Monthly report (sales, profit, expenses, net profit)
- Inventory report
- Repair summary
- Admin-only access

#### Database Backup & Restore
- Create timestamped SQL Server backups
- Browse available backups with file details
- Restore to new database instance
- Selectable backup location

### Technical Specifications

| Component | Technology |
|-----------|-----------|
| Framework | .NET 9 |
| UI | WPF with MaterialDesignInXamlToolkit 5.2.1 |
| Architecture | Clean Architecture (Domain, Application, Infrastructure, Desktop) |
| MVVM | CommunityToolkit.Mvvm 8.4.0 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server 2019+ / Express |
| Logging | Serilog with rolling file sink |
| Testing | xUnit 2.9.2 + Moq 4.20.72 |

### Known Issues

1. **Password change**: No built-in password change UI. Passwords must be changed via direct database update.
2. **User management**: No user creation/management UI beyond default seeding.
3. **Email receipt**: Email dialog exists but requires SMTP configuration.

### System Requirements

- **OS**: Windows 10/11 (64-bit)
- **Database**: SQL Server 2019+ or SQL Server Express
- **Runtime**: .NET 9 Desktop Runtime
- **RAM**: 4 GB minimum
- **Disk**: 500 MB application + database storage

### Installation

Run `KlayFixSetup_v1.0.0.exe` and follow the installation wizard. See the Administrator Guide for detailed installation instructions.

### Upgrade Notes

This is the initial release. No upgrade path from previous versions exists.

---

*Copyright (c) 2026 KlayFix Solutions*
