using KlayFix.Domain.Enums;

namespace KlayFix.Tests.Domain;

public class EnumTests
{
    [Fact]
    public void ProductCategory_HasExpectedValues()
    {
        Assert.Equal(0, (int)ProductCategory.Phone);
        Assert.Equal(1, (int)ProductCategory.Accessory);
        Assert.Equal(2, (int)ProductCategory.Tablet);
        Assert.Equal(3, (int)ProductCategory.Other);
    }

    [Fact]
    public void ProductCategory_ToString_ReturnsName()
    {
        Assert.Equal("Phone", ProductCategory.Phone.ToString());
        Assert.Equal("Accessory", ProductCategory.Accessory.ToString());
        Assert.Equal("Tablet", ProductCategory.Tablet.ToString());
        Assert.Equal("Other", ProductCategory.Other.ToString());
    }

    [Fact]
    public void RepairStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)RepairStatus.Received);
        Assert.Equal(1, (int)RepairStatus.InProgress);
        Assert.Equal(2, (int)RepairStatus.WaitingParts);
        Assert.Equal(3, (int)RepairStatus.Completed);
        Assert.Equal(4, (int)RepairStatus.Delivered);
    }

    [Fact]
    public void RepairStatus_OrderingIsCorrect()
    {
        var statuses = Enum.GetValues<RepairStatus>();
        Assert.Equal(RepairStatus.Received, statuses[0]);
        Assert.Equal(RepairStatus.InProgress, statuses[1]);
        Assert.Equal(RepairStatus.WaitingParts, statuses[2]);
        Assert.Equal(RepairStatus.Completed, statuses[3]);
        Assert.Equal(RepairStatus.Delivered, statuses[4]);
    }

    [Fact]
    public void PaymentMethod_HasExpectedValues()
    {
        Assert.Equal(0, (int)PaymentMethod.Cash);
        Assert.Equal(1, (int)PaymentMethod.Card);
    }

    [Fact]
    public void ExpenseCategory_HasExpectedValues()
    {
        Assert.Equal(0, (int)ExpenseCategory.Rent);
        Assert.Equal(1, (int)ExpenseCategory.Utilities);
        Assert.Equal(2, (int)ExpenseCategory.Supplies);
        Assert.Equal(3, (int)ExpenseCategory.Maintenance);
        Assert.Equal(4, (int)ExpenseCategory.Salary);
        Assert.Equal(5, (int)ExpenseCategory.Marketing);
        Assert.Equal(6, (int)ExpenseCategory.Other);
    }

    [Fact]
    public void PaymentMethod_ParseFromString()
    {
        var cash = Enum.Parse<PaymentMethod>("Cash", true);
        var card = Enum.Parse<PaymentMethod>("Card", true);

        Assert.Equal(PaymentMethod.Cash, cash);
        Assert.Equal(PaymentMethod.Card, card);
    }

    [Fact]
    public void RepairStatus_ParseFromString()
    {
        var received = Enum.Parse<RepairStatus>("Received", true);
        var inProgress = Enum.Parse<RepairStatus>("InProgress", true);
        var completed = Enum.Parse<RepairStatus>("Completed", true);

        Assert.Equal(RepairStatus.Received, received);
        Assert.Equal(RepairStatus.InProgress, inProgress);
        Assert.Equal(RepairStatus.Completed, completed);
    }

    [Fact]
    public void ExpenseCategory_ParseFromString()
    {
        var utilities = Enum.Parse<ExpenseCategory>("Utilities", true);
        var rent = Enum.Parse<ExpenseCategory>("Rent", true);

        Assert.Equal(ExpenseCategory.Utilities, utilities);
        Assert.Equal(ExpenseCategory.Rent, rent);
    }
}
