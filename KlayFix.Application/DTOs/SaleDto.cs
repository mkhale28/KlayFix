namespace KlayFix.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal Profit { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public List<SaleItemDto> Items { get; set; } = [];
}

public class SaleItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateSaleDto
{
    public decimal Discount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public List<CreateSaleItemDto> Items { get; set; } = [];
}

public class CreateSaleItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductBrand { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal PurchasePrice { get; set; }
}
