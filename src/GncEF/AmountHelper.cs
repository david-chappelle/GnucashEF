namespace GncEF
{
	public static class AmountHelper
	{
		public static (long num, long denom) FromString(string amt, long? desiredFraction = null)
		{
			amt = amt.Replace("$", string.Empty).Replace(",", string.Empty);
			long num, denom;

			var ndx = amt.IndexOf('.');
			if (ndx != -1)
			{
				// convert to a ratio of integers
				num = long.Parse(amt.Replace(".", string.Empty));
				denom = Pow(10, amt.Length - ndx - 1);
			}
			else
			{
				// no decimal point
				num = long.Parse(amt);
				denom = 1;
			}
			
			if (desiredFraction.HasValue)
				return Normalize(num, denom, desiredFraction.Value);

			return (num, denom);
		}

		/// <summary>
		/// Adds two fractions represented as ratios of two longs
		/// </summary>
		/// <param name="a">The first fraction to add.</param>
		/// <param name="b">The second fraction to add.</param>
		/// <param name="desiredDenominator">The desired denominator for the result, if possible.</param>
		/// <returns>A new fraction representing the sum. The denominator is the least common multiple of the input denominators.</returns>
		public static (long num, long denom) Add((long num, long denom) a, (long num, long denom) b, long? desiredDenominator = null)
		{
			long outNum, outDenom;

			if (a.denom == b.denom)
			{
				outNum = a.num + b.num;
				outDenom = a.denom;
			}
			else
			{
				outDenom = LCM(a.denom, b.denom);
				outNum = ((outDenom / a.denom) * a.num) + ((outDenom / b.denom) * b.num);
			}

			return desiredDenominator.HasValue ? Normalize(outNum, outDenom, desiredDenominator.Value) : (outNum, outDenom);
		}

		public static (long num, long denom) Normalize(long num, long denom, long desiredDenominator)
		{
			// For zero, just assign the desired denominator
			if (num == 0)
				return (num, desiredDenominator);

			// if the ratio can be expressed exactly with the desired denominator, do so, otherwise leave it alone
			if (denom == desiredDenominator)
				return (num, denom);
			else if (denom < desiredDenominator)
				return desiredDenominator % denom == 0 ? (num * (desiredDenominator / denom), desiredDenominator) : (num, denom);
			else
				return denom % desiredDenominator == 0 ? (num / (denom / desiredDenominator), desiredDenominator) : (num, denom);
		}

		/// <summary>
		/// Converts the ratio by forcing the desiredDenominator, introducting rounding errors if necessary. Use Normalize() if you want to avoid rounding errors.
		/// </summary>
		public static (long num, long denom) NormalizeForced(long num, long denom, long desiredDenominator = 100)
		{
			if (num == 0 || denom == desiredDenominator)
				return (num, desiredDenominator);

			long normalizedNum = checked((long) (long.BigMul(num, desiredDenominator) / denom));
			return (normalizedNum, desiredDenominator);
		}

		public static bool IsEquivalentTo(this (long num, long denom) a, (long num, long denom) b)
		{
			return AreEquivalent(a, b);
		}

		public static bool IsOffsetTo(this (long num, long denom) a, (long num, long denom) b)
		{
			return AreOffsetting(a, b);
		}

		public static bool AreEquivalent((long num, long denom) a, (long num, long denom) b)
		{
			// check for strict equality
			if (a == b)
				return true;

			// check for equivalence using cross multiplication
			return a.num * b.denom == b.num * a.denom;
		}

		public static bool AreOffsetting((long num, long denom) a, (long num, long denom) b)
		{
			// check for strict negation
			if (a == (-b.num,b.denom))
				return true;

			// check for negation equivalance using cross multiplication
			return a.num * b.denom == -b.num * a.denom;
		}

		private static long Pow(long x, long y)
		{
			long val = 1;
			for (int i = 0; i < y; i++)
				val *= x;

			return val;
		}

		/// <summary>
		/// Calculates the greatest common divisor (GCD) of two long integers using the Euclidean algorithm.
		/// </summary>
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

		/// <summary>
		/// Calculates the least common multiple (LCM) of two long integers.
		/// </summary>
		private static long LCM(long a, long b)
		{
			return (a / GCD(a, b)) * b;
		}
	}
}
