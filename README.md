# KlayFix ERP

Enterprise Desktop ERP System for Mobile Phone Shops (POS + Inventory + Repair Management + Expenses + Reporting)

## Tech Stack

- **.NET 9** WPF Desktop Application
- **Entity Framework Core 9** (Code First)
- **SQL Server** (LocalDB / Express)
- **MVVM** with CommunityToolkit.Mvvm
- **Material Design in XAML** (MaterialDesignThemes 5.x)
- **Dependency Injection** (Microsoft.Extensions.DependencyInjection)

## Architecture (Clean Architecture)

```
KlayFix.sln
├── KlayFix.Domain          # Entities + Enums only
├── KlayFix.Application     # Business logic, Services, DTOs, Interfaces
├── KlayFix.Infrastructure  # EF Core, DbContext, Repositories
└── KlayFix.Desktop         # WPF UI (MVVM only, no business logic)
```

## Modules

| Module | Description |
|--------|-------------|
| Dashboard | Animated metric cards (Sales, Profit, Repairs, Low Stock) |
| Products | Full CRUD with search, DataGrid, modal forms |
| Point of Sale | Two-panel POS layout with cart, discounts, payment methods |
| Repairs | Card-based repair tracking with status lifecycle |
| Expenses | Date-range filtered expense management |
| Reports | Daily Sales, Monthly Profit, Inventory, Repair Summary |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (installed with Visual Studio) or SQL Server Express

## Getting Started

### 1. Clone & Build

```bash
git clone <repo-url>
cd KlayFix
dotnet restore
dotnet build
```

### 2. Configure Connection String

Edit `KlayFix.Desktop/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=KlayFixDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=KlayFixDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Database (choose one)

**Option A: Auto-create** (no migration needed)
The app calls `EnsureCreated()` on first run — database is created automatically.

**Option B: EF Core Migrations**

```bash
dotnet ef database update --project KlayFix.Infrastructure --startup-project KlayFix.Desktop
```

To add a new migration:

```bash
dotnet ef migrations add MigrationName --project KlayFix.Infrastructure --startup-project KlayFix.Desktop
```

### 4. Run

```bash
dotnet run --project KlayFix.Desktop
```

Or set `KlayFix.Desktop` as startup project in Visual Studio and press **F5**.

## Design System

| Token | Value |
|-------|-------|
| Primary | `#2563EB` (Blue) |
| Sidebar | `#111827` (Dark) |
| Success | `#10B981` (Green) |
| Danger | `#EF4444` (Red) |
| Warning | `#F59E0B` (Amber) |
| Background | `#F3F4F6` (Gray 100) |
| Surface | `#FFFFFF` (White) |

- Flat design with rounded corners (8px–16px)
- Soft drop shadows on cards
- Hover scale effects on buttons
- Smooth sidebar navigation transitions
- Material Design icons (via MaterialDesignThemes)

## Project Structure

```
KlayFix.Desktop/
├── Views/
│   ├── MainWindow.xaml          # Shell with sidebar + top bar
│   └── Pages/
│       ├── DashboardPage.xaml   # Animated metric cards
│       ├── ProductsPage.xaml    # CRUD DataGrid + modal
│       ├── SalesPage.xaml       # POS layout (products + cart)
│       ├── RepairsPage.xaml     # Card-based tickets
│       ├── ExpensesPage.xaml    # Filterable table
│       └── ReportsPage.xaml     # Daily/Monthly/Inventory reports
├── ViewModels/                  # Strict MVVM (CommunityToolkit.Mvvm)
├── Converters/                  # Status colors, currency, date, bool
├── Styles/
│   ├── Colors.xaml              # Design tokens
│   └── ModernStyles.xaml        # All control styles
├── appsettings.json             # Connection string config
└── App.xaml / App.xaml.cs       # DI container + startup
```
