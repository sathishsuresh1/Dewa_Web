using DEWAXP.Foundation.Content.Models;
using System;

namespace DEWAXP.Feature.Account.Models.PrimaryAccount
{
	[Serializable]
	public class PrimaryAccountWorkflowState
	{
		public SharedAccount Account { get; set; }

		public bool Succeeded { get; set; }

		public string Message { get; set; }
	}
}