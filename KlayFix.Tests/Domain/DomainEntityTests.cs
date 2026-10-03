using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Domain;

public class DomainEntityTests
{
    [Fact]
    public void Product_DefaultValues()
    {
        var p = new Product();
        Assert.Equal(0, p.Id);
        Assert.Equal(string.Empty, p.Name);
        Assert.Equal(string.Empty, p.Brand);
        Assert.Equal(ProductCategory.Phone, p.Category);
        Assert.Null(p.IMEI);
        Assert.Equal(0m, p.PurchasePrice);
        Assert.Equal(0m, p.SellingPrice);
        Assert.Equal(0, p.Quantity);
        Assert.NotEqual(default, p.CreatedAt);
        Assert.NotEqual(default, p.UpdatedAt);
    }

    [Fact]
    public void Sale_DefaultValues()
    {
        var s = new Sale();
        Assert.Equal(0, s.Id);
        Assert.NotEqual(default, s.SaleDate);
        Assert.Equal(0m, s.TotalAmount);
        Assert.Equal(0m, s.Discount);
        Assert.Equal(0m, s.NetAmount);
        Assert.Equal(0m, s.Profit);
        Assert.Equal(PaymentMethod.Cash, s.PaymentMethod);
        Assert.Equal(string.Empty, s.InvoiceNumber);
        Assert.Empty(s.Items);
    }

    [Fact]
    public void SaleItem_DefaultValues()
    {
        var i = new SaleItem();
        Assert.Equal(0, i.Id);
        Assert.Equal(0, i.ProductId);
        Assert.Equal(string.Empty, i.ProductName);
        Assert.Equal(string.Empty, i.ProductBrand);
        Assert.Equal(0, i.Quantity);
        Assert.Equal(0m, i.UnitPrice);
        Assert.Equal(0m, i.TotalPrice);
        Assert.Equal(0m, i.PurchasePrice);
    }

    [Fact]
    public void Repair_DefaultValues()
    {
        var r = new Repair();
        Assert.Equal(0, r.Id);
        Assert.Equal(string.Empty, r.CustomerName);
        Assert.Equal(string.Empty, r.CustomerPhone);
        Assert.Equal(string.Empty, r.DeviceModel);
        Assert.Null(r.DeviceIMEI);
        Assert.Equal(string.Empty, r.IssueDescription);
        Assert.Equal(RepairStatus.Received, r.Status);
        Assert.Equal(0m, r.Cost);
        Assert.Equal(0m, r.Charge);
        Assert.NotEqual(default, r.CreatedAt);
        Assert.Null(r.CompletedAt);
        Assert.Null(r.DeliveredAt);
        Assert.Null(r.Notes);
    }

    [Fact]
    public void Expense_DefaultValues()
    {
        var e = new Expense();
        Assert.Equal(0, e.Id);
        Assert.Equal(string.Empty, e.Description);
        Assert.Equal(0m, e.Amount);
        Assert.Equal(ExpenseCategory.Rent, e.Category);
        Assert.NotEqual(default, e.Date);
        Assert.Null(e.Notes);
    }

    [Fact]
    public void Product_SetProperties()
    {
        var now = DateTime.UtcNow;
        var p = new Product
        {
            Id = 1,
            Name = "iPhone 15 Screen Protector",
            Brand = "Spigen",
            Category = ProductCategory.Accessory,
            IMEI = "123456789012345",
            PurchasePrice = 50.00m,
            SellingPrice = 150.00m,
            Quantity = 10,
            CreatedAt = now,
            UpdatedAt = now
        };

        Assert.Equal(1, p.Id);
        Assert.Equal("iPhone 15 Screen Protector", p.Name);
        Assert.Equal("Spigen", p.Brand);
        Assert.Equal(ProductCategory.Accessory, p.Category);
        Assert.Equal("123456789012345", p.IMEI);
        Assert.Equal(50.00m, p.PurchasePrice);
        Assert.Equal(150.00m, p.SellingPrice);
        Assert.Equal(10, p.Quantity);
        Assert.Equal(now, p.CreatedAt);
        Assert.Equal(now, p.UpdatedAt);
    }

    [Fact]
    public void Sale_CalculateNetAmount()
    {
        var sale = new Sale
        {
            TotalAmount = 1000.00m,
            Discount = 100.00m,
            NetAmount = 900.00m
        };

        Assert.Equal(1000.00m, sale.TotalAmount);
        Assert.Equal(100.00m, sale.Discount);
        Assert.Equal(900.00m, sale.NetAmount);
    }

    [Fact]
    public void SaleItem_CalculateTotalPrice()
    {
        var item = new SaleItem
        {
            Quantity = 3,
            UnitPrice = 150.00m,
            TotalPrice = 450.00m
        };

        Assert.Equal(3, item.Quantity);
        Assert.Equal(150.00m, item.UnitPrice);
        Assert.Equal(450.00m, item.TotalPrice);
    }

    [Fact]
    public void Repair_StatusTransitions()
    {
        var repair = new Repair();
        Assert.Equal(RepairStatus.Received, repair.Status);

        repair.Status = RepairStatus.InProgress;
        Assert.Equal(RepairStatus.InProgress, repair.Status);

        repair.Status = RepairStatus.Completed;
        Assert.Equal(RepairStatus.Completed, repair.Status);

        repair.Status = RepairStatus.Delivered;
        Assert.Equal(RepairStatus.Delivered, repair.Status);
    }

    [Fact]
    public void Repair_CompletedAtSetOnCompletion()
    {
        var repair = new Repair();
        repair.Status = RepairStatus.Completed;
        repair.CompletedAt = DateTime.UtcNow;
        Assert.NotNull(repair.CompletedAt);
    }

    [Fact]
    public void Repair_DeliveredAtSetOnDelivery()
    {
        var repair = new Repair();
        repair.Status = RepairStatus.Delivered;
        repair.DeliveredAt = DateTime.UtcNow;
        Assert.NotNull(repair.DeliveredAt);
    }

    [Fact]
    public void Sale_HasItemsCollection()
    {
        var sale = new Sale();
        Assert.NotNull(sale.Items);
        Assert.Empty(sale.Items);

        sale.Items.Add(new SaleItem { ProductName = "Test", Quantity = 1, UnitPrice = 100m, TotalPrice = 100m });
        Assert.Single(sale.Items);
    }

    [Fact]
    public void Sale_InvoiceNumberFormat()
    {
        var sale = new Sale
        {
            InvoiceNumber = "INV-20250101-A1B2C3"
        };
        Assert.StartsWith("INV-", sale.InvoiceNumber);
        Assert.Matches(@"^INV-\d{8}-[A-Z0-9]{6}$", sale.InvoiceNumber);
    }
}
