using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    public class AccountDetailsViewModel
    {
        public string BusinessPartner { get; set; }

        public bool loggedinuser { get; set; }

        public bool owner { get; set; }

        public bool Moveinindicator { get; set; }

        public string UserId { get; set; }

	    public string SessionToken { get; set; }

        public string[] PremiseAccount { get; set; }

        public bool personbusinesspartner { get; set; }

        public bool organisationbusinesspartner { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string CustomerCategory { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string AccountType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string IdType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string CustomerType { get; set; }

        [Foundation.DataAnnotations.Required(AllowEmptyStrings = false, ValidationMessageKey = "EnterValue")]
        public string PremiseNo { get; set; }

        public string IdNumber { get; set; }

        public string SecurityDeposit { get; set; }

        public string ReconnectionRegistrationFee { get; set; }

        public string AddressRegistrationFee { get; set; }

        public string ReconnectionVATrate { get; set; }

        public string ReconnectionVATamt { get; set; }

        public string AddressVATrate { get; set; }

        public string AddressVAtamt { get; set; }

        public string OutstandingBalance { get; set; }

        public DateTime? MoveinStartDate { get; set; }

        public DateTime? DocumentExpiryDate { get; set; }

        public string Reference { get; set; }
        public string KnowledgeFee { get; set; }
        public string InnovationFee { get; set; }

        public string ContractAccont { get; set; }

        public int NumberOfRooms { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase VatDocument { get; set; }

        public string VatNumber { get; set; }
    }
}
