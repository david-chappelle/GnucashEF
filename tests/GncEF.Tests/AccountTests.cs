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
        Assert.Equal((8750L, 100L), currentBalance);
    }

    [Fact]
    public void AccountValueWithRange()
    {
        var account = _db.AccountFromAbsolutePath("Expenses:Dining");
        Assert.NotNull(account);
        
        // as of today
        var currentBalance = _db.GetAccountValue(account, asOfDate: null);
        Assert.Equal((5337L, 100L), currentBalance);

        // as of 2026-08-01
        var bal2 = _db.GetAccountValue(account, asOfDate: new DateOnly(2026,8,1));
        Assert.Equal((2593L, 100L), bal2);
    }

    [Fact]
    public void AccountValueChangeByMonth()
    {
        // change in value for 2026-08
        var startDate = new DateOnly(2026,8,1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var expectedSpending = (4087L, 100L);        

        var accountDining = _db.AccountFromAbsolutePath("Expenses:Dining");
        Assert.NotNull(accountDining);
        var amtSpent = _db.GetAccountValueChange(accountDining, startDate, endDate);
        Assert.True(AmountHelper.AreEquivalent(expectedSpending, amtSpent));

        var accountCard = _db.AccountFromAbsolutePath("Liabilities:Credit Card");
        Assert.NotNull(accountCard);
        var amtCardChange = _db.GetAccountValueChange(accountCard, startDate, endDate);
        Assert.True(AmountHelper.AreOffsetting(expectedSpending, amtCardChange));

        Assert.True(AmountHelper.AreOffsetting(amtSpent, amtCardChange));
     }

    public void Dispose()
    {
        _db?.Dispose();
    }
}