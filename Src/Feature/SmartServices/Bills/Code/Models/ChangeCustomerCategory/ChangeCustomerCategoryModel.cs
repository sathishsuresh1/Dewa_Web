using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.ChangeCustomerCategory
{
	public class ChangeCustomerCategoryModel
	{
		public string ContractAccountNumber { get; set; }
		public string BusinessPartnerNumber { get; set; }
		public string PremiseNumber { get; set; }


		[Required(AllowEmptyStrings = false, ValidationMessageKey = "uae mobile number validation message")]
		public string Mobile { get; set; }

		[Required(AllowEmptyStrings = false, ValidationMessageKey = "generic validation message")]
		public string Description { get; set; }

		public HttpPostedFileBase IdUploader { get; set; }
	}
}