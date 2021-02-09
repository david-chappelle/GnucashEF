using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GnucashLib.Tests
{
	public class AccountTests
	{
		[Fact]
		public void ReadAllAccounts()
		{
			using (var context = new GnucashContext(@"D:\db\test.gnucash"))
			{
				var accounts = context.Accounts.Include(a => a.Commodity).ToArray();
			}
		}
	}
}
