using System.Runtime;
using System.Text;

namespace GncEF.Models;

public record struct Ratio(long Num, long Denom)
{
    public static Ratio operator +(Ratio a, Ratio b)
    {
        return Add(a, b);
    }
    
    public static Ratio operator -(Ratio a, Ratio b)
    {
        return Subtract(a, b);
    }
    
    public void operator +=(Ratio other)
    {
        this = Add(this, other);
    }
    
    public void operator -=(Ratio other)
    {
        this = Subtract(this, other);
    }

    public static Ratio FromString(string amt, long? desiredDenominator = null, bool trimForUsd = true)
    {
        // remove whitespace
        string fmtAmt = string.Concat(amt.Where(c => !char.IsWhiteSpace(c)));

        if (trimForUsd)
        {
            var charsToIgnore = new[] { '$', ',' };
            fmtAmt = string.Concat(fmtAmt.Where(c => !charsToIgnore.Contains(c)));
        }

        Ratio ratio;
        var ndx = fmtAmt.IndexOf('.');
        if (ndx != -1)
        {
            // convert to a ratio of integers
            long num = long.Parse(fmtAmt.Replace(".", string.Empty));
            long denom = 1;
            for (int i = 0; i < fmtAmt.Length - ndx - 1; i++)
                denom *= 10;
            ratio = new Ratio(num, denom);
        }
        else
        {
            // no decimal point
            ratio = new Ratio(long.Parse(fmtAmt), 1);
        }

        return desiredDenominator.HasValue ? ratio.Normalize(desiredDenominator.Value) : ratio;
    }
    
    public static Ratio Add(Ratio a, Ratio b, long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        Ratio sum;
        if (a.Denom == b.Denom)
            sum = new Ratio(a.Num + b.Num, a.Denom);
        else
        {
            var lcm = LCM(a.Denom, b.Denom);
            var outNum = ((lcm / a.Denom) * a.Num) + ((lcm / b.Denom) * b.Num);

            sum = new Ratio(outNum, lcm);
        }
        
        return desiredDenominator.HasValue ?
            sum.Normalize(desiredDenominator.Value, allowRoundoffError) :
            sum;
    }

    public static Ratio Subtract(Ratio a, Ratio b, long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        Ratio sum;
        if (a.Denom == b.Denom)
            sum = new Ratio(a.Num - b.Num, a.Denom);
        else
        {
            var lcm = LCM(a.Denom, b.Denom);
            var outNum = ((lcm / a.Denom) * a.Num) - ((lcm / b.Denom) * b.Num);

            sum = new Ratio(outNum, lcm);
        }
        
        return desiredDenominator.HasValue ?
            sum.Normalize(desiredDenominator.Value, allowRoundoffError) :
            sum;        
    }

    public Ratio Normalize(long desiredDenominator, bool allowRoundoffError = false)
    {
        // if denominator matches or invalid, nothing to do
        if (Denom == desiredDenominator || Denom == 0)
            return this;

        // for zero, just assign the desired denominator, if necessary
        if (Num == 0)
            return new Ratio(0, desiredDenominator);

        // if forced, then convert the ratio even if there is a roundoff error
        // if not forced, then only convert the ratio if there is no roundoff error
        if (allowRoundoffError ||
            (Denom < desiredDenominator && desiredDenominator % Denom == 0) ||
            (Denom > desiredDenominator && Denom % desiredDenominator == 0))
        {
            return new Ratio(checked((long)(long.BigMul(Num, desiredDenominator) / Denom)), desiredDenominator);
        }
        
        // no roundoffs allowed and ratio cannot be converted without roundoff, so do nothing
        return this;
    }
    
    public bool IsEquivalentTo(Ratio other)
    {
        if (this == other)
            return true;
        
        // check for equivalence using cross multiplication
        return Num * other.Denom == other.Num * Denom;        
    }
    
    public bool IsOppositeOf(Ratio other)
    {
        if (Num == -other.Num && Denom == other.Denom)
            return true;
            
        // check for negation using cross multiplication
        return Num * other.Denom == -other.Num * Denom;
    }

    public decimal ToDecimal()
    {
        return Num / (decimal)Denom;
    }

    private static long GCD(long a, long b)
    {
        if (a < 0) a = -a;
        if (b < 0) b = -b;

        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
    
    private static long LCM(long a, long b)
    {
        return (a / GCD(a, b)) * b;
    }        
};