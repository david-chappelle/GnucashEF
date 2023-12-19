using System;

namespace GnucashLib
{
	public static class DateHelper
	{
		public static DateOnly? FromYYYYMMDD(this string d)
		{
			DateOnly r;
			return DateOnly.TryParseExact(d, "yyyyMMdd", out r) ? r : null;
		}

		public static DateTime? FromNormalDT(this string d)
		{
			DateTime dt;
			return DateTime.TryParse(d, out dt) ? dt : null;
		}
	}
}
