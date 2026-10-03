using Moq;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Services;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Application;

public class RepairServiceTests
{
    private readonly Mock<IRepairRepository> _mockRepo;
    private readonly RepairService _service;

    public RepairServiceTests()
    {
        _mockRepo = new Mock<IRepairRepository>();
        _service = new RepairService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRepairs()
    {
        var repairs = new List<Repair>
        {
            new() { Id = 1, CustomerName = "John", DeviceModel = "iPhone 15", Status = RepairStatus.Received, Cost = 100m, Charge = 200m, CreatedAt = DateTime.UtcNow }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(repairs);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("John", result[0].CustomerName);
        Assert.Equal("iPhone 15", result[0].DeviceModel);
        Assert.Equal("Received", result[0].Status);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsRepair()
    {
        var repair = new Repair { Id = 1, CustomerName = "Jane", DeviceModel = "Samsung S24", IssueDescription = "Broken screen", Status = RepairStatus.InProgress };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(repair);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Jane", result!.CustomerName);
        Assert.Equal("Broken screen", result.IssueDescription);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Repair?)null);
        var result = await _service.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesRepairWithReceivedStatus()
    {
        var dto = new CreateRepairDto
        {
            CustomerName = "Alice",
            CustomerPhone = "09171234567",
            DeviceModel = "Pixel 8",
            DeviceIMEI = "123456789012345",
            IssueDescription = "Battery issue",
            Cost = 50m,
            Charge = 150m,
            Notes = "Under warranty"
        };

        Repair? captured = null;
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Repair>()))
            .Callback<Repair>(r => captured = r)
            .ReturnsAsync((Repair r) => r);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("Alice", captured.CustomerName);
        Assert.Equal("09171234567", captured.CustomerPhone);
        Assert.Equal("Pixel 8", captured.DeviceModel);
        Assert.Equal("123456789012345", captured.DeviceIMEI);
        Assert.Equal("Battery issue", captured.IssueDescription);
        Assert.Equal(RepairStatus.Received, captured.Status);
        Assert.Equal(50m, captured.Cost);
        Assert.Equal(150m, captured.Charge);
        Assert.Equal("Under warranty", captured.Notes);
        Assert.NotEqual(default, captured.CreatedAt);

        Assert.Equal("Alice", result.CustomerName);
        Assert.Equal("Received", result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToCompleted_SetsCompletedAt()
    {
        var repair = new Repair { Id = 1, Status = RepairStatus.InProgress, CreatedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(repair);

        var dto = new UpdateRepairStatusDto { Id = 1, Status = "Completed" };

        Repair? captured = null;
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Repair>()))
            .Callback<Repair>(r => captured = r)
            .ReturnsAsync((Repair r) => r);

        await _service.UpdateStatusAsync(dto);

        Assert.Equal(RepairStatus.Completed, captured!.Status);
        Assert.NotNull(captured.CompletedAt);
        Assert.Null(captured.DeliveredAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToDelivered_SetsDeliveredAt()
    {
        var repair = new Repair { Id = 1, Status = RepairStatus.Completed, CompletedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(repair);

        var dto = new UpdateRepairStatusDto { Id = 1, Status = "Delivered" };

        Repair? captured = null;
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Repair>()))
            .Callback<Repair>(r => captured = r)
            .ReturnsAsync((Repair r) => r);

        await _service.UpdateStatusAsync(dto);

        Assert.Equal(RepairStatus.Delivered, captured!.Status);
        Assert.NotNull(captured.DeliveredAt);
        Assert.NotNull(captured.CompletedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_ToWaitingParts_NoTimestamps()
    {
        var repair = new Repair { Id = 1, Status = RepairStatus.Received, CreatedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(repair);

        var dto = new UpdateRepairStatusDto { Id = 1, Status = "WaitingParts" };

        Repair? captured = null;
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Repair>()))
            .Callback<Repair>(r => captured = r)
            .ReturnsAsync((Repair r) => r);

        await _service.UpdateStatusAsync(dto);

        Assert.Equal(RepairStatus.WaitingParts, captured!.Status);
        Assert.Null(captured.CompletedAt);
        Assert.Null(captured.DeliveredAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithNotes_UpdatesNotes()
    {
        var repair = new Repair { Id = 1, Status = RepairStatus.Received, CreatedAt = DateTime.UtcNow };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(repair);

        var dto = new UpdateRepairStatusDto { Id = 1, Status = "InProgress", Notes = "Awaiting parts from supplier" };

        Repair? captured = null;
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Repair>()))
            .Callback<Repair>(r => captured = r)
            .ReturnsAsync((Repair r) => r);

        await _service.UpdateStatusAsync(dto);

        Assert.Equal("Awaiting parts from supplier", captured!.Notes);
    }

    [Fact]
    public async Task UpdateStatusAsync_NonExisting_Throws()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Repair?)null);
        var dto = new UpdateRepairStatusDto { Id = 99, Status = "Completed" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateStatusAsync(dto));
    }

    [Fact]
    public async Task GetByStatusAsync_ReturnsFiltered()
    {
        var repairs = new List<Repair>
        {
            new() { Id = 1, CustomerName = "Bob", DeviceModel = "Xiaomi", Status = RepairStatus.Received }
        };
        _mockRepo.Setup(r => r.GetByStatusAsync("Received")).ReturnsAsync(repairs);

        var result = await _service.GetByStatusAsync("Received");

        Assert.Single(result);
        Assert.Equal("Received", result[0].Status);
    }

    [Fact]
    public async Task GetByStatusAsync_NoMatch_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.GetByStatusAsync("Delivered")).ReturnsAsync([]);
        var result = await _service.GetByStatusAsync("Delivered");
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatching()
    {
        var repairs = new List<Repair>
        {
            new() { Id = 1, CustomerName = "Charlie", DeviceModel = "Oppo Find X", IssueDescription = "Charging port issue" }
        };
        _mockRepo.Setup(r => r.SearchAsync("Oppo")).ReturnsAsync(repairs);

        var result = await _service.SearchAsync("Oppo");

        Assert.Single(result);
        Assert.Equal("Oppo Find X", result[0].DeviceModel);
    }

    [Fact]
    public async Task SearchAsync_NoMatch_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.SearchAsync("xyz")).ReturnsAsync([]);
        var result = await _service.SearchAsync("xyz");
        Assert.Empty(result);
    }
}
