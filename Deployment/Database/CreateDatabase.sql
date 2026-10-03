-- ============================================
-- KlayFix ERP Database Creation Script
-- Version: 1.0.0
-- Description: Creates KlayFixDB database with all tables, indexes, and default data
-- Compatible with: SQL Server 2019+, SQL Server Express, LocalDB
-- ============================================

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'KlayFixDB')
BEGIN
    CREATE DATABASE [KlayFixDB]
    ON PRIMARY (
        NAME = N'KlayFixDB',
        FILENAME = N'$(data_path)\KlayFixDB.mdf',
        SIZE = 10MB,
        FILEGROWTH = 10MB
    )
    LOG ON (
        NAME = N'KlayFixDB_log',
        FILENAME = N'$(data_path)\KlayFixDB_log.ldf',
        SIZE = 5MB,
        FILEGROWTH = 5MB
    );
    PRINT 'Database KlayFixDB created successfully.';
END
ELSE
    PRINT 'Database KlayFixDB already exists.';
GO

USE [KlayFixDB];
GO

-- ============================================
-- TABLES
-- ============================================

-- Users table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [PasswordHash] NVARCHAR(200) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table Users created.';
END
GO

-- Products table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(200) NOT NULL,
        [Brand] NVARCHAR(100) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [IMEI] NVARCHAR(50) NULL,
        [PurchasePrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [SellingPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Quantity] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table Products created.';
END
GO

-- Sales table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Sales] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SaleDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [InvoiceNumber] NVARCHAR(50) NOT NULL,
        [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Discount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [NetAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Profit] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [PaymentMethod] NVARCHAR(20) NOT NULL,
        CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table Sales created.';
END
GO

-- SaleItems table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleItems]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[SaleItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [SaleId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [ProductName] NVARCHAR(200) NOT NULL,
        [ProductBrand] NVARCHAR(100) NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18,2) NOT NULL,
        [TotalPrice] DECIMAL(18,2) NOT NULL,
        [PurchasePrice] DECIMAL(18,2) NOT NULL,
        CONSTRAINT [PK_SaleItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_SaleItems_Sales] FOREIGN KEY ([SaleId])
            REFERENCES [dbo].[Sales] ([Id])
            ON DELETE CASCADE
    );
    PRINT 'Table SaleItems created.';
END
GO

-- Repairs table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Repairs]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Repairs] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CustomerName] NVARCHAR(200) NOT NULL,
        [CustomerPhone] NVARCHAR(50) NOT NULL,
        [DeviceModel] NVARCHAR(200) NOT NULL,
        [DeviceIMEI] NVARCHAR(50) NULL,
        [IssueDescription] NVARCHAR(2000) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT N'Received',
        [Cost] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Charge] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CompletedAt] DATETIME2 NULL,
        [DeliveredAt] DATETIME2 NULL,
        [Notes] NVARCHAR(2000) NULL,
        CONSTRAINT [PK_Repairs] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table Repairs created.';
END
GO

-- Expenses table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Expenses]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Expenses] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Description] NVARCHAR(500) NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [Date] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [Notes] NVARCHAR(2000) NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Table Expenses created.';
END
GO

-- ============================================
-- INDEXES
-- ============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username')
    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users]([Username]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_SaleDate')
    CREATE NONCLUSTERED INDEX [IX_Sales_SaleDate] ON [dbo].[Sales]([SaleDate]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sales_InvoiceNumber')
    CREATE UNIQUE INDEX [IX_Sales_InvoiceNumber] ON [dbo].[Sales]([InvoiceNumber]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SaleItems_SaleId')
    CREATE NONCLUSTERED INDEX [IX_SaleItems_SaleId] ON [dbo].[SaleItems]([SaleId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Category')
    CREATE NONCLUSTERED INDEX [IX_Products_Category] ON [dbo].[Products]([Category]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Repairs_Status')
    CREATE NONCLUSTERED INDEX [IX_Repairs_Status] ON [dbo].[Repairs]([Status]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Expenses_Date')
    CREATE NONCLUSTERED INDEX [IX_Expenses_Date] ON [dbo].[Expenses]([Date]);
GO

PRINT 'All indexes created.';
GO

-- ============================================
-- DEFAULT DATA - USERS
-- ============================================

-- Password: Admin@123 (PBKDF2 SHA-256 hash)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = N'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [FullName], [Role], [IsActive], [CreatedAt])
    VALUES (N'admin', N'<PASSWORD_HASH_PLACEHOLDER>', N'Administrator', N'Admin', 1, GETUTCDATE());
    PRINT 'Default admin user created.';
END
GO

-- Password: Sales@123 (PBKDF2 SHA-256 hash)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = N'sales')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [FullName], [Role], [IsActive], [CreatedAt])
    VALUES (N'sales', N'<PASSWORD_HASH_PLACEHOLDER>', N'Sales Person', N'Salesman', 1, GETUTCDATE());
    PRINT 'Default sales user created.';
END
GO

PRINT '============================================';
PRINT 'KlayFixDB database deployment complete.';
PRINT '============================================';
GO
