using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.Account.Models.UpdatePrimaryContact
{
    public class UpdatePrimaryContactModel : GenericPageWithIntro
    {
        [Required(ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string EmailAddress { get; set; }

        [Required(ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobileNumber { get; set; }
    }
}