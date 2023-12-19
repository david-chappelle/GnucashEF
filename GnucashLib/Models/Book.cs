namespace GnucashLib.Models
{
	public class Book
	{
		public string BookId { get; set; }
		public string RootAccountId { get; set; }
		public string RootTemplateId { get; set; }

		public virtual Account RootAccount { get; set; }
		public virtual Account RootTemplateAccount { get; set; }
	}
}
