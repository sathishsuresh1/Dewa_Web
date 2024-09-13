
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    public class ContactDetailsViewModel
    {
        //[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Owner { get; set; }

        //[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        //[Foundation.DataAnnotations.RegularExpression("^[0-9]+$", ValidationMessageKey = "J10.P0BoxValidation")]
        public string POBox { get; set; }

        [Foundation.DataAnnotations.Required]
        public int NumberOfRooms { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string Emirate { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid UAE mobile number")]
        [Foundation.DataAnnotations.RegularExpression(@"^(?:0)?(?:50|51|52|53|54|55|56|57|58|59)\d{7}$", ValidationMessageKey = "Please enter a valid UAE mobile number")]
        public string MobilePhone { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "Please enter a valid email address")]
       // [Foundation.DataAnnotations.EmailAddress(ValidationMessageKey = "Please enter a valid email address")]
        public string Email { get; set; }

        public string CustomerType { get; set; }

        //[Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string Nationality { get; set; }

        public bool DetailsReadonly { get; set; }

        public string NationalalityText  { get; set; }

        public string PassportorEmiratesLabel { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase IdentityDocument { get; set; }

        public string IdentityDocumentLabel1 { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase OwnerDocument { get; set; }

        public string OwnerDocumentLabel { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase TitleDeed { get; set; }

        public string TitleDeedLabel { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase TradeLicense { get; set; }

        public string TradeLicenseLabel { get; set; }

        public bool AttachmentFlag { get; set; }
    }
}
