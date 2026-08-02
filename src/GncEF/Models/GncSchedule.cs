using System;
using System.Collections.Generic;
using System.Text;

namespace GncEF.Models
{
	public class GncSchedule
	{
		public int ScheduleId { get; set; }
		public string ObjectId { get; set; }
		public int Frequency { get; set; }
		public string Period { get; set; }
		public string Start { get; set; }
		public string WeekendAdjustment { get; set; }

		public virtual GncScheduledTransaction ScheduledTransaction { get; set; }
	}
}
