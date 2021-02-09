using System;
using System.Collections.Generic;
using System.Text;

namespace GnucashLib
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

			// check for normalization needed to desired denominator
			if (desiredFraction.HasValue && desiredFraction.Value != denom)
			{
				num = (num * desiredFraction.Value) / denom;
				denom = desiredFraction.Value;
			}

			return (num, denom);
		}

		public static long Pow(long x, long y)
		{
			long val = 1;
			for (int i = 0; i < y; i++)
				val *= x;

			return val;
		}
	}
}
