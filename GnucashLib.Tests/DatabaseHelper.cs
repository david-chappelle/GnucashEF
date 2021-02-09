namespace GnucashLib.Tests
{
	public class DatabaseHelper
	{
		public static GnucashContext GetContext()
		{
			return new GnucashContext(@"D:\db\test.gnucash");
		}
	}
}
