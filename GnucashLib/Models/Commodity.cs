namespace GnucashLib.Models
{
	public class Commodity
	{
		public string CommodityId { get; set; }
		public string Namespace { get; set; }
		public string Mnenomic { get; set; }
		public string FullName { get; set; }
		public string Cusip { get; set; }
		public int Fraction { get; set; }
		public bool QuoteFlag { get; set; }
		public string QuoteSource { get; set; }
		public string QuoteTimezone { get; set; }
	}
}
