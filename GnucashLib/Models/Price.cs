using System;

namespace GnucashLib.Models
{
	public class Price
	{
		public string PriceId { get; set; }
		public string CommodityId { get; set; }
		public string CurrencyId { get; set; }
		public DateTime Date { get; set; }
		public string Source { get; set; }
		public string Type { get; set; }
		public long ValueNumerator { get; set; }
		public long ValueDenominator { get; set; }

		public virtual Commodity Commodity { get; set; }
		public virtual Commodity Currency { get; set; }
	}
}
