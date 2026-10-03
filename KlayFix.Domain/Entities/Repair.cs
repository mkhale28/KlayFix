using KlayFix.Domain.Enums;

namespace KlayFix.Domain.Entities;

public class Repair
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string? DeviceIMEI { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public RepairStatus Status { get; set; } = RepairStatus.Received;
    public decimal Cost { get; set; }
    public decimal Charge { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? Notes { get; set; }
}
