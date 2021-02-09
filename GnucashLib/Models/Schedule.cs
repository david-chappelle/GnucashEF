using System;
using System.Collections.Generic;
using System.Text;

namespace GnucashLib.Models
{
	public class Schedule
	{
		public int ScheduleId { get; set; }
		public string ObjectId { get; set; }
		public int Frequency { get; set; }
		public string Period { get; set; }
		public string Start { get; set; }
		public string WeekendAdjustment { get; set; }

		public virtual ScheduledTransaction ScheduledTransaction { get; set; }
	}
}
