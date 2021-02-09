using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GnucashLib.Tests
{
	public class PriceTests
	{
		[Fact]
		public void ReadAllPrices()
		{
			using (var context = DatabaseHelper.GetContext())
			{
				var prices = context.Prices
					.Include(p => p.Commodity)
					.Include(p => p.Currency)
					.ToArray();
			}
		}

		[Fact]
		public void ReadTspPrices()
		{
			using (var context = DatabaseHelper.GetContext())
			{
				var tspPrices = context.Prices.Where(p => p.Commodity.Mnenomic == "TSPL2040").ToArray();
			}
		}
	}
}
