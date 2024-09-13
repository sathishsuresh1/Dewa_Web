using System.ComponentModel.DataAnnotations;
using System.Web;
using DEWAXP.Foundation.Integration.Enums;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Models.Common;

namespace DEWAXP.Feature.SupplyManagement.Models.Complaints
{
    [Bind(Include = "AccountNumber,MobileNumber,EmailAddress,ComplaintAttachmentUploader,RequestCategory,RequestCategory,ComplaintType,Description,AgreedToPayment")]
    public class BillingComplaint : GenericPageWithIntro
	{
		[Required]
		public string AccountNumber { get; set; }

		[Required]
		public string MobileNumber { get; set; }

		[Required]
		public string EmailAddress { get; set; }

		// Optional
		public HttpPostedFileBase ComplaintAttachmentUploader { get; set; }

		[Required]
		public ComplaintPriority RequestCategory { get; set; }

		[Required]
		public MunicipalService ComplaintType { get; set; }	

		[Required]
		public string Description { get; set; }

		[Required]
		public bool AgreedToPayment { get; set; }
	}
}