using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Payment
{
	public abstract class PaymentRequestModel
    {
		public abstract PaymentContext PaymentContext { get; }

		public virtual bool IsValid()
		{
			return true;
		}
    }
}