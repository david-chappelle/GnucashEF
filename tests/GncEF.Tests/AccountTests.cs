using GncEF.Models;

namespace GncEF.Tests;

public class AccountTests : IDisposable
{
    private readonly GncContext _db = new GncContext(Path.Combine("data", "Test.gnucash"));

    [Fact]
    public void AccountValue()
    {
        var account = _db.AccountFromAbsolutePath("Assets:Current Assets:Cash in Wallet");
        Assert.NotNull(account);
        
        var currentBalance = _db.GetAccountValue(account);
        var expectedBalance = new Ratio(8750, 100);
        Assert.Equal(expectedBalance, currentBalance);
    }

    [Fact]
    public void AccountValueWithRange()
    {
        var account = _db.AccountFromAbsolutePath("Expenses:Dining");
        Assert.NotNull(account);
        
        // as of today
        var currentBalance = _db.GetAccountValue(account, asOfDate: null);
        var expectedBalance = new Ratio(5337, 100);
        Assert.Equal(expectedBalance, currentBalance);

        // as of 2026-08-01
        var bal2 = _db.GetAccountValue(account, asOfDate: new DateOnly(2026,8,1));
        var expectedBal2 = new Ratio(2593, 100);
        Assert.Equal(expectedBal2, bal2);
    }

    [Fact]
    public void AccountValueChangeByMonth()
    {
        // change in value for 2026-08
        var startDate = new DateOnly(2026,8,1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var expectedSpending = new Ratio(4087, 100);        

        var accountDining = _db.AccountFromAbsolutePath("Expenses:Dining");
        Assert.NotNull(accountDining);
        var amtSpent = _db.GetAccountValueChange(accountDining, startDate, endDate);
        Assert.True(expectedSpending.IsEquivalentTo(amtSpent));;

        var accountCard = _db.AccountFromAbsolutePath("Liabilities:Credit Card");
        Assert.NotNull(accountCard);
        var amtCardChange = _db.GetAccountValueChange(accountCard, startDate, endDate);
        Assert.True(expectedSpending.IsOppositeOf(amtCardChange));

        Assert.True(amtSpent.IsOppositeOf(amtCardChange));
     }

    public void Dispose()
    {
        _db?.Dispose();
    }
}