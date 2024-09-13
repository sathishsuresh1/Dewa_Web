using System;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Feature.SupplyManagement.ModelBinders;
using DEWAXP.Foundation.Content.Models.Payment;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    [ModelBinder(typeof(MoveInTenancyModelBinder))]
    public class TenancyDetailsViewModel
    {
        public DateTime? ContractStartDate { get; set; }
         
        public DateTime? ContractEndDate { get; set; }

       
        [Foundation.DataAnnotations.Required]
        public int NumberOfRooms { get; set; }

        //[Foundation.DataAnnotations.Required(AllowEmptyStrings = false)]
        //[Foundation.DataAnnotations.MaxLength(9, ValidationMessageKey = "J10.ContractValueMessage")]
        [Foundation.DataAnnotations.RegularExpression(@"^*[0-9,\.]+$", ValidationMessageKey = "J10.ContractValueMessage")]
        public string ContractValue { get; set; }

        [Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase UploadContract { get; set; }

		[Foundation.DataAnnotations.MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
		public HttpPostedFileBase UploadContractCopy { get; set; }

		public string ContractLabel1 { get; set; }

		public string ContractLabel2 { get; set; }

		public string CustomerType { get; set; }

		public bool MultipleDocumentsRequired { get; set; }

        public double SecurityDeposit { get; set; }
        public PaymentMethod paymentMethod { get; set; }

        public string bankkey { get;set; }
        public string SuqiaDonation { get; set; }
        public string SuqiaDonationAmt { get; set; }
    }
}