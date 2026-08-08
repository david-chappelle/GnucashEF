namespace GncEF.Models
{
	public class GncPrice
	{
		public string PriceId { get; set; }
		public string CommodityId { get; set; }
		public string CurrencyId { get; set; }
		public DateTime Date { get; set; }
		public string Source { get; set; }
		public string Type { get; set; }
		public long ValueNumerator { get; set; }
		public long ValueDenominator { get; set; }

		public decimal Value => decimal.Divide(ValueNumerator, ValueDenominator);

		public virtual GncCommodity Commodity { get; set; }
		public virtual GncCommodity Currency { get; set; }
	}
}
