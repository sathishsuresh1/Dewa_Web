using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DEWAXP.Feature.Account.Models.CustomerRegistration
{
	public class BusinessPartnerNumberModel
	{
		[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "register business partner validation message")]
		[Foundation.DataAnnotations.MaxLength(10, ValidationMessageKey = "register business partner validation message")]
		public string BusinessPartnerNumber { get; set; }
	}
}