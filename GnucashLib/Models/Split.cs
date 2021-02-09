using System;

namespace GnucashLib.Models
{
	public class Split
	{
		public string SplitId { get; set; }
		public string TransactionId { get; set; }
		public string AccountId { get; set; }
		public string Memo { get; set; }
		public string Action { get; set; }
		public string ReconcileState { get; set; }
		public DateTime ReconcileDate { get; set; }
		public long ValueNumerator { get; set; }
		public long ValueDenominator { get; set; }
		public long QuantityNumerator { get; set; }
		public long QuantityDenominator { get; set; }
		public string LotId { get; set; }

		public virtual Transaction Transaction { get; set; }
		public virtual Account Account { get; set; }
		// TODO: lot
	}
}
