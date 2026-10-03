using Microsoft.EntityFrameworkCore;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Repair> Repairs => Set<Repair>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Brand).HasMaxLength(100).IsRequired();
            entity.Property(p => p.IMEI).HasMaxLength(50);
            entity.Property(p => p.PurchasePrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Category).HasConversion<string>().HasMaxLength(50);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(s => s.Discount).HasColumnType("decimal(18,2)");
            entity.Property(s => s.NetAmount).HasColumnType("decimal(18,2)");
            entity.Property(s => s.Profit).HasColumnType("decimal(18,2)");
            entity.Property(s => s.PaymentMethod).HasConversion<string>().HasMaxLength(20);
            entity.Property(s => s.InvoiceNumber).HasMaxLength(50).IsRequired();
            entity.HasMany(s => s.Items).WithOne(i => i.Sale).HasForeignKey(i => i.SaleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(i => i.ProductBrand).HasMaxLength(100);
            entity.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(i => i.PurchasePrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.Property(r => r.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(r => r.CustomerPhone).HasMaxLength(50).IsRequired();
            entity.Property(r => r.DeviceModel).HasMaxLength(200).IsRequired();
            entity.Property(r => r.DeviceIMEI).HasMaxLength(50);
            entity.Property(r => r.IssueDescription).HasMaxLength(2000).IsRequired();
            entity.Property(r => r.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(r => r.Cost).HasColumnType("decimal(18,2)");
            entity.Property(r => r.Charge).HasColumnType("decimal(18,2)");
            entity.Property(r => r.Notes).HasMaxLength(2000);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });
    }
}
