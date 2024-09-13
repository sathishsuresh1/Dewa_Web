using DEWAXP.Foundation.DataAnnotations;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class ForgotUsername
    {
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ValidationMessageKey = "email validation message")]
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string Email { get; set; }
    }
}