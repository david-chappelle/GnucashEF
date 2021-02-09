using System;

namespace GnucashLib.Models
{
	public class Slot
	{
		public int SlotId { get; set; }
		public string ObjectId { get; set; }
		public string Name { get; set; }
		public int Type { get; set; }
		public long LongVal { get; set; }
		public string StringVal { get; set; }
		public double DoubleVal { get; set; }
		public DateTime DateTimeVal { get; set; }
		public string IdVal { get; set; }
		public long NumeratorVal { get; set; }
		public long DenominatorVal { get; set; }
		public DateTime DateVal { get; set; }
	}
}
