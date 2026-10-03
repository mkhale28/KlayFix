# KlayFix ERP - User Guide

## Version 1.0.0

---

## Table of Contents

1. [Introduction](#introduction)
2. [Login](#login)
3. [Dashboard](#dashboard)
4. [Products Management](#products-management)
5. [Point of Sale (Sales)](#point-of-sale)
6. [Repairs Management](#repairs-management)
7. [Expenses](#expenses)
8. [Reports](#reports)
9. [Database Backup & Restore](#database-backup-restore)
10. [Language Switching](#language-switching)
11. [Keyboard Shortcuts](#keyboard-shortcuts)

---

## Introduction

KlayFix ERP is a desktop application for managing mobile phone repair shops. It provides tools for inventory management, point-of-sale operations, repair tracking, expense tracking, and business reporting.

### System Requirements

- **Operating System**: Windows 10 or Windows 11 (64-bit)
- **Database**: SQL Server 2019+ or SQL Server Express
- **Memory**: 4 GB RAM minimum (8 GB recommended)
- **Disk Space**: 500 MB for application, additional space for database

---

## Login

1. Launch KlayFix ERP from the Start Menu or desktop shortcut.
2. The login screen will appear.
3. Enter your username and password.
4. Click **Login** or press **Enter**.

### Default Credentials

| Role | Username | Password |
|------|----------|----------|
| Administrator | `admin` | `Admin@123` |
| Sales Person | `sales` | `Sales@123` |

> **Important**: Change the default password after your first login.

---

## Dashboard

The Dashboard provides an overview of your business performance.

### Key Metrics

- **Sales Today**: Total sales amount for the current day
- **Profit Today**: Net profit from today's sales
- **Invoices**: Number of sales transactions today
- **Repairs Overview**: Count of repairs by status (New, Active, Done)
- **Low Stock Alerts**: Products with stock below threshold
- **Expenses Today**: Today's operational expenses

> Note: Dashboard is only available to Admin users.

---

## Products Management

The Products page allows you to manage your inventory.

### Adding a Product

1. Navigate to **Products** in the sidebar.
2. Click **+ Add Product**.
3. Fill in the product details:
   - **Product Name** (required)
   - **Brand** (required)
   - **Category** (Phone, Accessory, Tablet, Part, Other)
   - **IMEI/Serial** (optional)
   - **Cost Price**
   - **Selling Price**
   - **Stock Quantity**
4. Click **Save**.

### Editing a Product

1. Find the product in the list.
2. Click the **Edit** button (pencil icon).
3. Update the fields.
4. Click **Save**.

### Deleting a Product

1. Click the **Delete** button (trash icon).
2. Confirm the deletion.

### Searching Products

Use the search bar to find products by name, brand, or IMEI number.

---

## Point of Sale

The Point of Sale (POS) page is used to process customer sales.

### Making a Sale

1. Navigate to **Point of Sale** in the sidebar.
2. Browse products in the product grid, or use the search bar.
3. Click on a product to add it to the cart.
4. Adjust quantities using **+** and **-** buttons.
5. Apply a discount if needed (Admin only).
6. Select payment method: **Cash** or **Card**.
7. Click **Complete Sale**.

### Using the Barcode Scanner

1. Focus the scanner input field (blue bar at top).
2. Scan a product barcode or IMEI.
3. The product is automatically added to the cart.

### Printing a Receipt

After completing a sale:
1. Click **Print Receipt**.
2. Select a printer from the Windows print dialog.
3. Click **Print**.

### Email Receipt

1. After completing a sale, click **Email Receipt**.
2. Enter the customer's email address.
3. Click **Send**.

---

## Repairs Management

The Repairs page tracks device repairs from check-in to delivery.

### Creating a Repair Ticket

1. Navigate to **Repairs** in the sidebar.
2. Click **+ New Repair**.
3. Enter customer details:
   - **Customer Name** (required)
   - **Customer Phone** (required)
   - **Device Model** (required)
   - **IMEI** (optional)
   - **Issue Description** (required)
   - **Cost** (parts cost)
   - **Charge** (service fee)
   - **Notes** (optional)
4. Click **Create Ticket**.

### Updating Repair Status

1. Find the repair ticket.
2. Click **Advance Status** to move to the next status.
   - Status flow: Received → In Progress → Waiting Parts → Completed → Delivered

### Searching Repairs

Use the search bar to find repairs by customer name, phone, device, or IMEI.

### Filtering by Status

Use the status filter dropdown to filter repairs by their current status.

---

## Expenses

The Expenses page tracks business operational costs.

### Adding an Expense

1. Navigate to **Expenses** in the sidebar.
2. Click **+ Add Expense**.
3. Enter:
   - **Description** (required)
   - **Amount** (required, must be greater than 0)
   - **Category** (Rent, Utilities, Supplies, Maintenance, Salary, Marketing, Other)
   - **Date**
   - **Notes** (optional)
4. Click **Save**.

### Filtering Expenses

Set a date range using the **From** and **To** date pickers, then click **Filter**.

> Note: Expenses page is only available to Admin users.

---

## Reports

The Reports page provides business analytics.

### Available Reports

1. **Daily Sales Report**: View sales totals for a specific date.
2. **Monthly Report**: View monthly sales, profit, expenses, and net profit.
3. **Inventory Report**: View all products with their stock levels.
4. **Repair Summary**: View all repair records.

> Note: Reports page is only available to Admin users.

---

## Database Backup & Restore

### Creating a Backup

1. Navigate to **Backup** in the sidebar.
2. Click **Create Backup**.
3. Select the destination folder.
4. Wait for the backup to complete.
5. A confirmation message will appear with the backup file location.

### Restoring a Backup

1. Navigate to **Backup** in the sidebar.
2. Select a backup file from the list.
3. Click **Restore Selected Backup**.
4. Confirm the restore operation.
5. A restored copy of the database will be created with a timestamp suffix.

> **Note**: Restoring creates a new database copy. The original database is not affected. To use the restored database, update the connection string.

---

## Language Switching

KlayFix ERP supports both English and Arabic.

1. In the sidebar footer, click **English** to switch to English.
2. Click **العربية** to switch to Arabic.

The interface will immediately update with the selected language.

---

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Enter** (on login) | Submit login |
| **Ctrl+F** (on Sales) | Focus scanner input |
| **Ctrl+N** | New item (Product/Repair/Expense) |
| **Esc** | Close modal dialog |

---

## Support

For technical support, contact your system administrator.

---

*Document version: 1.0.0*
*Last updated: June 25, 2026*
