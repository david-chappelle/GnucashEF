namespace GnucashLib.Models
{
	public class GncBook
	{
		public string BookId { get; set; }
		public string RootAccountId { get; set; }
		public string RootTemplateId { get; set; }

		public virtual GncAccount RootAccount { get; set; }
		public virtual GncAccount RootTemplateAccount { get; set; }
	}
}
