using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace GnucashLib.Tests
{
	public class CommodityTests
	{
		[Fact]
		public void ReadAllCommodities()
		{
			using (var context = DatabaseHelper.GetContext())
			{
				var commodities = context.Commodities.ToArray();
			}
		}
	}
}
