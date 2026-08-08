namespace GncEF;

public static class DateHelper
{
	// ReSharper disable once InconsistentNaming
	public static DateOnly? FromYYYYMMDD(this string d)
	{
		return DateOnly.TryParseExact(d, "yyyyMMdd", out var r) ? r : null;
	}

	public static DateTime? FromNormalDateTime(this string d)
	{
		return DateTime.TryParse(d, out var dt) ? dt : null;
	}
}