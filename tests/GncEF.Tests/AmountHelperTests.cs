using Xunit;
using GncEF;
using GncEF.Models;

namespace GncEF.Tests;

public class AmountHelperTests
{
    [Fact]
    public void TestAddNoDenominator()
    {
        var a = new Ratio(1,2);
        var b = new Ratio(1,3);

        var sum = a + b;
        Assert.Equal(new Ratio(5, 6), sum);        
    }

    [Fact]
    public void TestAddNoDenominator2()
    {
        var a = new Ratio(1, 10);
        var b = new Ratio(7, 100);

        var sum = a+b;
        var expectedSum = new Ratio(17, 100);
        Assert.Equal(expectedSum, sum);        
    }

    [Fact]
    public void TestAddWithDenominatorNoNormalize()
    {
        var a = new Ratio(1, 10);
        var b = new Ratio(7, 100);

        var sum = Ratio.Add(a, b, desiredDenominator: 100L);
        var expectedSum = new Ratio(17, 100);
        Assert.Equal(expectedSum, sum);        
    }

    [Fact]
    public void TestAddWithDenominatorNormalizeNoRemainder()
    {
        var a = new Ratio(1, 10);;
        var b = new Ratio(10, 100);

        var sum = Ratio.Add(a, b, desiredDenominator: 100L);
        var expectedSum = new Ratio(20, 100);
        Assert.Equal(expectedSum, sum);        
    }

    [Fact]
    public void TestAddWithDenominatorNormalizeWithRemainder()
    {
        var a = new Ratio(17, 10);
        var b = new Ratio(12, 100);

        var sum = Ratio.Add(a, b, desiredDenominator: 100L);
        var expectedSum = new Ratio(182, 100);
        Assert.Equal(expectedSum, sum);        
    }    
}
