using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.Subscribe
{
	[Serializable]
	public class SubscribeResponseModel
	{
		public string Settings { get; set; }
		public string Email { get; set; }
	}
}