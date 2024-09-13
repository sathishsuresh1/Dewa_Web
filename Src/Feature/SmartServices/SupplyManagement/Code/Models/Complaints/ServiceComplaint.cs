using DEWAXP.Foundation.Content.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.Complaints
{
    [Bind(Include = "City,AccountNumber,ContactName,MobileNumber,Description,File,ComplaintType,xGPS,yGPS,LocationCoordinates")]
    public class ServiceComplaint : GenericPageWithIntro
	{
		// Only required when not logged in
		public string City { get; set; }

		// Only required when logged in
		public string AccountNumber { get; set; }

		// Only required when not logged in
		public string ContactName { get; set; }

		// Only required when logged in
		public string MobileNumber { get; set; }

		[Required]
		public string Description { get; set; }

		// Optional
		public HttpPostedFileBase File { get; set; }

		[Required]
		public string ComplaintType { get; set; }

		public string xGPS { get; set; }

		public string yGPS { get; set; }

		public bool LocationCoordinates { get; set; }
	}
}