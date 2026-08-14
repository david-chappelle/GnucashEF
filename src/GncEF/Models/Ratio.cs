namespace GncEF.Models;

public readonly record struct Ratio(long Numerator, long Denominator)
{
    public bool Equals(Ratio other)
    {
        // check for exact equivalence
        if (this.Numerator == other.Numerator && this.Denominator == other.Denominator)
            return true;
        
        // check for denormalized equivalence using cross multiplication
        return Numerator * other.Denominator == other.Numerator * Denominator;        
    }

    public override int GetHashCode()
    {
        // return the hash code of the reduced ratio
        // this will ensure (1/2) and (2/4) have the same hash code
        var reduced = Reduce(Numerator, Denominator);
        return HashCode.Combine(reduced.n, reduced.d);
    }
    
    public static Ratio operator +(Ratio a, Ratio b) => Add(a, b);
    public static Ratio operator -(Ratio a, Ratio b) => Subtract(a, b);
    public static Ratio operator *(Ratio a, Ratio b) => Multiply(a, b);
    public static Ratio operator /(Ratio a, Ratio b) => Divide(a, b);
    public static Ratio operator -(Ratio a) => a with { Numerator = -a.Numerator };
    
    public static Ratio ParseUsd(string amt)
    {
        // remove whitespace
        const string charsToTrimForUsd = "$,";
        string trimmedAmt = string.Concat(amt.Where(c => !char.IsWhiteSpace(c) && !charsToTrimForUsd.Contains(c)));

        // this uses US style decimal point
        Ratio ratio;
        var ndx = trimmedAmt.IndexOf('.');
        if (ndx != -1)
        {
            // convert to a ratio of integers
            long num = long.Parse(trimmedAmt.Replace(".", string.Empty));
            int pow = trimmedAmt.Length - ndx - 1;
            if (pow > PowersOfTen.Length - 1)
                throw new ArgumentException("Fraction too many digits");

            ratio = new Ratio(num, PowersOfTen[pow]);
        }
        else
        {
            // no decimal point present
            ratio = new Ratio(long.Parse(trimmedAmt), 1);
        }

        // make base 100 without rounding in case we're given $1,384.5400
        return ratio.Normalize(100);
    }
    
    /// <summary>
    /// Add two ratios
    /// </summary>
    /// <param name="a">First ratio</param>
    /// <param name="b">Second ratio</param>
    /// <param name="desiredDenominator">Desired denominator for the sum, or null for no preference (will use LCM)</param>
    /// <param name="allowRoundoffError">If true and <paramref name="desiredDenominator"/> is not null, force the denominator even with roundoff error</param>
    /// <returns></returns>
    public static Ratio Add(Ratio a, Ratio b, long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        Ratio sum;
        if (a.Denominator == b.Denominator)
            sum = a with { Numerator = a.Numerator + b.Numerator };
        else
        {
            var lcm = LCM(a.Denominator, b.Denominator);
            var outNum = ((lcm / a.Denominator) * a.Numerator) + ((lcm / b.Denominator) * b.Numerator);

            sum = new Ratio(outNum, lcm);
        }
        
        return sum.Normalize(desiredDenominator, allowRoundoffError);
    }

    public static Ratio Subtract(Ratio a, Ratio b, long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        return Add(a,-b,desiredDenominator,allowRoundoffError);
    }

    public static Ratio Multiply(Ratio a, Ratio b, long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        return new Ratio(checked(a.Numerator * b.Numerator), checked(a.Denominator * b.Denominator))
            .Normalize(desiredDenominator, allowRoundoffError);
    }
    
    public static Ratio Divide(Ratio a, Ratio b, long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        return new Ratio(checked(a.Numerator * b.Denominator), checked(a.Denominator * b.Numerator))
            .Normalize(desiredDenominator, allowRoundoffError);
    }

    /// <summary>
    /// Convert to a new ratio with the specified denominator
    /// </summary>
    /// <param name="desiredDenominator">Denominator of the new ratio, or null for smallest available</param>
    /// <param name="allowRoundoffError">if true, convert even if the new ratio cannot be represented exactly.</param>
    /// <returns>New ratio with the specified denominator. If allowRoundoffError is false and there is a roundoff, this current ratio is returned instead.</returns>
    public Ratio Normalize(long? desiredDenominator = null, bool allowRoundoffError = false)
    {
        // divide by zero, don't attempt normalization
        if (Denominator == 0)
            return this;
        
        // if denominator supplied and matches, nothing to do
        if (Denominator == desiredDenominator)
            return this;

        // for zero, just assign the desired denominator, or 1 if none supplied
        if (Numerator == 0)
            return new Ratio(0, desiredDenominator ?? 1);

        long newNumerator, newDenominator;
        if (desiredDenominator == null)
        {
            // no denominator specified, so use the smallest valid denominator that would not cause roundoff
            var reduced = Reduce(Numerator, Denominator);
            newNumerator = reduced.n;
            newDenominator = reduced.d;
        }
        else
        {
            // denominator specified, so convert the ratio to use it
            var dr = long.DivRem(checked(Numerator * desiredDenominator.Value), Denominator);
        
            // there is a roundoff and caller requires no error, so do nothing
            if (!allowRoundoffError && dr.Remainder != 0)
                return this;

            // handle rounding
            newNumerator =
                (dr.Remainder > 0 && dr.Remainder * 2 >= Denominator) ? dr.Quotient+1 :
                (dr.Remainder < 0 && -dr.Remainder * 2 >= Denominator) ? dr.Quotient-1 :
                dr.Quotient;
            
            newDenominator = desiredDenominator.Value;
        }

        return new Ratio(newNumerator, newDenominator);
    }
    
    public bool IsOppositeOf(Ratio other)
    {
        return this == -other;
    }

    public string ToUsdString()
    {
        return $"{ToDecimal():C}";
    }

    public decimal ToDecimal()
    {
        return Numerator / (decimal)Denominator;
    }
    
    private static (long n, long d) Reduce(long n, long d)
    {
        long gcd = GCD(n, d);
        return (n / gcd, d / gcd);
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

    private static int? log10Exact(long n)
    {
        if (n <= 0)
            return null;

        int p = 0;
        while (n % 10 == 0)
        {
            n /= 10;
            p++;
        }
        
        return n == 1 ? p : null;
    }
    
    private static readonly long[] PowersOfTen = [
        1, // 10^0 = 1
        10, // 10^1 = 10
        100, // 10^2 = 100
        1000, // 10^3 = 1,000
        10000, // 10^4 = 10,000
        100000, // 10^5 = 100,000
        1000000, // 10^6 = 1,000,000
        10000000, // 10^7 = 10,000,000
        100000000, // 10^8 = 100,000,000
        1000000000, // 10^9 = 1,000,000,000
        10000000000, // 10^10 = 10,000,000,000
        100000000000, // 10^11 = 100,000,000,000
        1000000000000, // 10^12 = 1,000,000,000,000
        10000000000000, // 10^13 = 10,000,000,000,000
        100000000000000, // 10^14 = 100,000,000,000,000
        1000000000000000, // 10^15 = 1,000,000,000,000,000
        10000000000000000, // 10^16 = 10,000,000,000,000,000
        100000000000000000, // 10^17 = 100,000,000,000,000,000
        1000000000000000000, // 10^18 = 1,000,000,000,000,000,000
    ];    
};