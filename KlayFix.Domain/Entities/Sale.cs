using KlayFix.Domain.Enums;

namespace KlayFix.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal Profit { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public List<SaleItem> Items { get; set; } = [];
}
