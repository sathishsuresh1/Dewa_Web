using DEWAXP.Foundation.Content;
using System.ComponentModel.DataAnnotations;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    public class CreateOnlineAccountModel
    {
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "register username validation message")]
        [Foundation.DataAnnotations.MinLength(6, ValidationMessageKey = "register username validation message")]
        [Foundation.DataAnnotations.MaxLength(75, ValidationMessageKey = "register username validation message")]
        public string UserId { get; set; }

        [DataType(DataType.Password)]
        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "login password validation message")]
        [Foundation.DataAnnotations.MinLength(8, ValidationMessageKey = "login password validation message")]
        [Foundation.DataAnnotations.RegularExpression(@"[^\w\d]*(([0-9]+.*[A-Za-z]+.*)|[A-Za-z]+.*([0-9]+.*))", ValidationMessageKey = DictionaryKeys.Global.Login.InvalidPassword)]
        [Foundation.DataAnnotations.NonEqual("UserId", ValidationMessageKey = DictionaryKeys.Global.Login.InvalidPassword)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Foundation.DataAnnotations.Compare("Password", ValidationMessageKey = "Password mismatch error")]
        public string ConfirmationPassword { get; set; }

        public string BusinessPartnerNumber { get; set; }

		public bool IsExistingCustomer { get; set; }

        public string Termslink { get; set; }
    }
}