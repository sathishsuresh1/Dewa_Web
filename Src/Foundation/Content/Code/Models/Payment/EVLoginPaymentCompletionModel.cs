using DEWAXP.Foundation.Integration.APIHandler.Models.Response;
using DEWAXP.Foundation.Integration.DewaSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Payment
{
	public class EVLoginPaymentCompletionModel : PaymentCompletionModel
	{
        public List<ContractAccount> AccountDetailsList { get; set; }
        public EVLoginPaymentCompletionModel(PaymentContext context, bool succeeded) 
			: base(context, succeeded)
		{
		}
	}
}