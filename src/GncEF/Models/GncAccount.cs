using System.Collections.Generic;

namespace GncEF.Models
{
	public class GncAccount
	{
		public string AccountId { get; set; }
		public string Name { get; set; }
		public string AccountType { get; set; }
		public string CommodityId { get; set; }
		public int CommodityFraction { get; set; }
		public bool NonStandardFraction { get; set; }
		public string ParentGuid { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public bool Hidden { get; set; }
		public bool Placeholder { get; set; }

		public virtual GncCommodity Commodity { get; set; }
		public virtual GncAccount ParentAccount { get; set; }
		public virtual ICollection<GncSlot> Slots { get; set; }
		public virtual ICollection<GncSplit> Splits { get; set; }
		public virtual ICollection<GncAccount> ChildAccounts { get; set; }
	}
}
