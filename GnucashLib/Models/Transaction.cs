using System;

namespace GnucashLib.Models
{
	public class Transaction
	{
		public string TransactionId { get; set; }
		public string CurrencyId { get; set; }
		public string Number { get; set; }
		public DateTime PostDate { get; set; }
		public DateTime EnteredDate { get; set; }
		public string Description { get; set; }

		public virtual Commodity Currency { get; set; }
	}
}
