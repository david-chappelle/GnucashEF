namespace GncEF.Models
{
	public class GncTransaction
	{
		public string TransactionId { get; set; }
		public string CurrencyId { get; set; }
		public string Number { get; set; }
		public DateTime PostDate { get; set; }
		public DateTime EnteredDate { get; set; }
		public string Description { get; set; }

		public virtual GncCommodity Currency { get; set; }
		public virtual ICollection<GncSplit> Splits { get; set; }
	}
}
