using System.Collections.Generic;

namespace GnucashLib.Models
{
	public class Account
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

		public virtual Commodity Commodity { get; set; }
		public virtual Account ParentAccount { get; set; }
		public virtual IEnumerable<Slot> Slots { get; set; }
		public virtual ICollection<Split> Splits { get; set; }
		public virtual ICollection<Account> ChildAccounts { get; set; }
	}
}
