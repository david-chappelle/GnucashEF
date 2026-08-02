using System;
using System.Collections.Generic;
using System.Text;

namespace GnucashLib.Models
{
	public class GncLot
	{
		public string LotId { get; set; }
		public string AccountId { get; set; }
		public bool IsClosed { get; set; }

		public virtual GncAccount Account { get; set; }
	}
}
