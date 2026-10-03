namespace KlayFix.Application.DTOs;

public class RepairDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string? DeviceIMEI { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public decimal Charge { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
}

public class CreateRepairDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string? DeviceIMEI { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public decimal Charge { get; set; }
    public string? Notes { get; set; }
}

public class UpdateRepairStatusDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
