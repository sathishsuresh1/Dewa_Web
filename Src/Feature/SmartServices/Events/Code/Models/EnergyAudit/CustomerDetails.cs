
using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.Events.Models.EnergyAudit
{
    public class CustomerDetails
    {
        public string CustomerName { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string ContactPerson { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string ContactMobile { get; set; }
        
        public string ContactTelephone { get; set; }

        [Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
        [EmailAddress(ValidationMessageKey = "Please enter a valid email address")]
        public string ContactEmail { get; set; }
        
        public string Address { get; set; }
    }
}
