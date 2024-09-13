using DEWAXP.Foundation.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveTo
{
	public class MoveToConfirmModel
    {
		public SharedAccount Account { get; set; }
		
		public bool IsSuccess { get; set; }

		public string Message { get; set; }

		public string ErrorMessage { get; set; }
	}
}