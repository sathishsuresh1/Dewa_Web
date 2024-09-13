using DEWAXP.Foundation.Content.Models.Payment;
using DEWAXP.Foundation.DataAnnotations;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.ClearanceCertificate
{
    public class ClearanceCertificatePropertySeller
    {
        public string BusinessPartnerNumber { get; set; }

        [Required]
        public string ContractAccountNumber { get; set; }

        public string ReceivedContractAccountNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }
        public string Amounts { get; set; }

        public string Remarks { get; set; }

        public decimal OutstandingBill { get; set; }

        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase AttachedDocument { get; set; }

        public PaymentMethod paymentMethod { get; set; }
        public string bankkey { get; set; }
        public string Languague { get; set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}