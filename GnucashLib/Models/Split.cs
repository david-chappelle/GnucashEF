using System;

namespace GnucashLib.Models
{
	public class Split
	{
		public string SplitId { get; set; }
		public string TransactionId { get; set; }
		public string AccountId { get; set; }
		public string Memo { get; set; }
		public string ActionName { get; set; }
		public string ReconcileState { get; set; }
		public DateTime? ReconcileDate { get; set; }
		public long ValueNumerator { get; set; }
		public long ValueDenominator { get; set; }
		public long QuantityNumerator { get; set; }
		public long QuantityDenominator { get; set; }
		public string LotId { get; set; }

		public decimal Value => decimal.Divide(ValueNumerator, ValueDenominator);
		public decimal Quantity => decimal.Divide(QuantityNumerator, QuantityDenominator);
		public ActionType? Action => ActionName switch
		{
			ACTION_SELL => ActionType.Sell,
			ACTION_BUY => ActionType.Buy,
			ACTION_FEE => ActionType.Fee,
			ACTION_DIVIDEND => ActionType.Dividend,
			_ => null
		};

		public virtual Transaction Transaction { get; set; }
		public virtual Account Account { get; set; }
		// TODO: lot

		public const string ACTION_SELL = "Sell";
		public const string ACTION_BUY = "Buy";
		public const string ACTION_FEE = "Fee";
		public const string ACTION_DIVIDEND = "Dividend";
	}

	public enum ActionType
	{
		Sell,
		Buy,
		Fee,
		Dividend
	}


}
