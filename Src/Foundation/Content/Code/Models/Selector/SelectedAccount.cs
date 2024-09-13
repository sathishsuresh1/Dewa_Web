namespace DEWAXP.Foundation.Content.Models
{
	public class SelectedAccount
	{
		[DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "No accounts available")]
		public string SelectedAccountNumber { get; set; }
	}
}