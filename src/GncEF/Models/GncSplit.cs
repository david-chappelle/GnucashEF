namespace GncEF.Models
{
	public class GncSplit
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

		public decimal Value => ValueRatio.ToDecimal();
		public decimal Quantity => QuantityRatio.ToDecimal();
		public Ratio ValueRatio => new Ratio(ValueNumerator, ValueDenominator);
		public Ratio QuantityRatio => new Ratio(QuantityNumerator, QuantityDenominator);
		public GncActionType? Action => ActionName switch
		{
			ACTION_SELL => GncActionType.Sell,
			ACTION_BUY => GncActionType.Buy,
			ACTION_FEE => GncActionType.Fee,
			ACTION_DIVIDEND => GncActionType.Dividend,
			_ => null
		};

		public virtual GncTransaction Transaction { get; set; }
		public virtual GncAccount Account { get; set; }
		// TODO: lot

		public const string ACTION_SELL = "Sell";
		public const string ACTION_BUY = "Buy";
		public const string ACTION_FEE = "Fee";
		public const string ACTION_DIVIDEND = "Dividend";
	}

	public enum GncActionType
	{
		Sell,
		Buy,
		Fee,
		Dividend
	}


}
