using System;
using Xunit;
using GnucashLib;
using System.Data;
using System.Linq;

namespace GnucashLib.Tests
{
	public class ModelTests
	{
		[Fact]
		public void Open()
		{
			IDbConnection db = null;

			try
			{
				db = GnucashLib.Database.FromSqlite(@"D:\db\david.gnucash");
				db.Open();
			}
			finally
			{
				db?.Close();
			}
		}
	}
}
