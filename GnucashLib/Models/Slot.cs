using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GnucashLib.Models
{
	public class Slot
	{
		public int SlotId { get; set; }
		public string ObjectId { get; set; }
		public string Name { get; set; }
		public int RawType { get; set; }
		public long? LongVal { get; set; }
		public string StringVal { get; set; }
		public double? DoubleVal { get; set; }
		public string DateTimeValRaw { get; set; }
		public string IdVal { get; set; }
		public long? NumeratorVal { get; set; }
		public long? DenominatorVal { get; set; }
		public string DateValRaw { get; set; }

		public SlotTypes? SlotType => Enum.IsDefined(typeof(SlotTypes), RawType) ? (SlotTypes?)RawType : null;
		public DateOnly? DateVal => DateValRaw.FromYYYYMMDD();
		public DateTime? DateTimeVal => DateTimeValRaw.FromNormalDT();

		public virtual Account Account { get; set; }

		public object Object => SlotType switch
		{
			SlotTypes.LongType => LongVal,
			SlotTypes.StringType when StringVal == "true" => true,
			SlotTypes.StringType when StringVal == "false" => false,
			SlotTypes.StringType => StringVal,
			SlotTypes.IdType => IdVal,
			SlotTypes.DateTimeType => DateTimeVal,
			SlotTypes.IdType2 => IdVal,
			SlotTypes.DateType => DateVal,
			_ => null
		};
	}

	public enum SlotTypes
	{
		LongType = 1,
		StringType = 4,
		IdType = 5,
		DateTimeType = 6,
		IdType2 = 9,
		DateType = 10
	}
}
