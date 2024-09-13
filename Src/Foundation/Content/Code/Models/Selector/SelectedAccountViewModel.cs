namespace DEWAXP.Foundation.Content.Models
{
	public class SelectedAccountViewModel
	{
		public SharedAccount Account { get; set; }

		public string FieldLabel { get; set; }

		public SelectedAccountViewModel(SharedAccount account)
		{
			Account = account;
		}
	}
}