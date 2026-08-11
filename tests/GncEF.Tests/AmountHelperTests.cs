using Xunit;
using GncEF;

namespace GncEF.Tests;

public class AmountHelperTests
{
    [Fact]
    public void TestAddNoDenominator()
    {
        var a = (num: 1L, denom: 2L);
        var b = (num: 1L, denom: 3L);

        var sum = a.Add(b);
        Assert.Equal((5L, 6L), sum);        
    }

    [Fact]
    public void TestAddNoDenominator2()
    {
        var a = (num: 1L, denom: 10L);
        var b = (num: 7L, denom: 100L);

        var sum = a.Add(b);
        Assert.Equal((17L, 100L), sum);        
    }

    [Fact]
    public void TestAddWithDenominatorNoNormalize()
    {
        var a = (num: 1L, denom: 10L);
        var b = (num: 7L, denom: 100L);

        var sum = a.Add(b, desiredDenominator: 100L);
        Assert.Equal((17L, 100L), sum);        
    }

    [Fact]
    public void TestAddWithDenominatorNormalizeNoRemainder()
    {
        var a = (num: 1L, denom: 10L);
        var b = (num: 10L, denom: 100L);

        var sum = a.Add(b, desiredDenominator: 100L);
        Assert.Equal((20L, 100L), sum);        
    }

    [Fact]
    public void TestAddWithDenominatorNormalizeWithRemainder()
    {
        var a = (num: 17L, denom: 10L);
        var b = (num: 12L, denom: 100L);

        var sum = a.Add(b, desiredDenominator: 100L);
        Assert.Equal((182L, 100L), sum);        
    }    
}
