using Moq;
using KlayFix.Application.DTOs;
using KlayFix.Application.Interfaces;
using KlayFix.Application.Services;
using KlayFix.Domain.Entities;
using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Application;

public class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _mockRepo;
    private readonly ExpenseService _service;

    public ExpenseServiceTests()
    {
        _mockRepo = new Mock<IExpenseRepository>();
        _service = new ExpenseService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllExpenses()
    {
        var expenses = new List<Expense>
        {
            new() { Id = 1, Description = "Electric Bill", Amount = 1500m, Category = ExpenseCategory.Utilities, Date = DateTime.Today }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expenses);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Electric Bill", result[0].Description);
        Assert.Equal(1500m, result[0].Amount);
        Assert.Equal("Utilities", result[0].Category);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsExpense()
    {
        var expense = new Expense { Id = 1, Description = "Rent", Amount = 10000m, Category = ExpenseCategory.Rent, Date = DateTime.Today };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expense);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Rent", result!.Description);
        Assert.Equal("Rent", result.Category);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Expense?)null);
        var result = await _service.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesExpense()
    {
        var dto = new CreateExpenseDto
        {
            Description = "Office Supplies",
            Amount = 2500m,
            Category = "Supplies",
            Date = new DateTime(2025, 1, 15),
            Notes = "Bought pens and paper"
        };

        Expense? captured = null;
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Expense>()))
            .Callback<Expense>(e => captured = e)
            .ReturnsAsync((Expense e) => e);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(captured);
        Assert.Equal("Office Supplies", captured.Description);
        Assert.Equal(2500m, captured.Amount);
        Assert.Equal(ExpenseCategory.Supplies, captured.Category);
        Assert.Equal(new DateTime(2025, 1, 15), captured.Date);
        Assert.Equal("Bought pens and paper", captured.Notes);

        Assert.Equal("Supplies", result.Category);
        Assert.Equal("Office Supplies", result.Description);
    }

    [Fact]
    public async Task CreateAsync_DefaultCategory_IsOther()
    {
        var dto = new CreateExpenseDto
        {
            Description = "Misc",
            Amount = 100m,
            Category = "Other"
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Expense>()))
            .ReturnsAsync((Expense e) => e);

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Other", result.Category);
    }

    [Fact]
    public async Task DeleteAsync_Existing_Deletes()
    {
        var expense = new Expense { Id = 1, Description = "Test", Amount = 100m };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expense);
        _mockRepo.Setup(r => r.DeleteAsync(expense)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _mockRepo.Verify(r => r.DeleteAsync(expense), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_Throws()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Expense?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(99));
    }

    [Fact]
    public async Task GetByDateRangeAsync_ReturnsFiltered()
    {
        var from = new DateTime(2025, 1, 1);
        var to = new DateTime(2025, 1, 31);
        var expenses = new List<Expense>
        {
            new() { Id = 1, Description = "Jan Rent", Amount = 10000m, Category = ExpenseCategory.Rent, Date = new DateTime(2025, 1, 5) }
        };
        _mockRepo.Setup(r => r.GetByDateRangeAsync(from, to)).ReturnsAsync(expenses);

        var result = await _service.GetByDateRangeAsync(from, to);

        Assert.Single(result);
        Assert.Equal("Jan Rent", result[0].Description);
    }

    [Fact]
    public async Task GetByDateRangeAsync_NoMatch_ReturnsEmpty()
    {
        _mockRepo.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([]);
        var result = await _service.GetByDateRangeAsync(DateTime.MinValue, DateTime.MaxValue);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTotalByDateAsync_ReturnsSum()
    {
        _mockRepo.Setup(r => r.GetTotalByDateAsync(new DateTime(2025, 1, 15))).ReturnsAsync(5000m);
        var total = await _service.GetTotalByDateAsync(new DateTime(2025, 1, 15));
        Assert.Equal(5000m, total);
    }

    [Fact]
    public async Task GetTotalByDateAsync_NoExpenses_ReturnsZero()
    {
        _mockRepo.Setup(r => r.GetTotalByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(0m);
        var total = await _service.GetTotalByDateAsync(DateTime.Today);
        Assert.Equal(0m, total);
    }
}
