using DEWAXP.Foundation.Content.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.PremiseTypeChange
{
    [Bind(Include = "ContractAccountNumber,MobileNumber,Remarks")]
    public class ChangePremiseType : GenericPageWithIntro
	{
		[Required]
		public string ContractAccountNumber { get; set; }

		[Required]
		public string MobileNumber { get; set; }

		public string PremiseNumber { get; set; }
		
		public string Remarks { get; set; }
	}
}