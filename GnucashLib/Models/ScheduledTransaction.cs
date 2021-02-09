using System;
using System.Collections.Generic;
using System.Text;

namespace GnucashLib.Models
{
	public class ScheduledTransaction
	{
		public string ScheduledTransactionId { get; set; }
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string LastOccurence { get; set; }
		public int NumberOccurences { get; set; }
		public int RemainingOccurences { get; set; }
		public bool AutoCreate { get; set; }
		public bool NotifyWhenCreated { get; set; }
		public int AdvancedCreationDays { get; set; }
		public int AdvancedNotifyDays { get; set; }
		public int Count { get; set; }
		public string TemplateId { get; set; }
	}
}
