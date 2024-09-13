using System;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Integration.Requests
{
	[Serializable]
	public class SubmitGovernmentObservation
	{
		public SubmitGovernmentObservation()
		{
			Attachments = new List<byte[]>();
		}

		public string MobileNumber { get; set; }

		public DateTime Date { get; set; }

		public string Area { get; set; }

		public string Road { get; set; }

		public string Structure { get; set; }
		
		public string ContactAccountNumber{ get; set; }

		public string Defect { get; set; }

		public string Email { get; set; }

		public string ElectricityOrWater { get; set; }

		public string Community{ get; set; }

		public string xGPS{ get; set; }

		public string yGPS { get; set; }
		
		public List<byte[]> Attachments { get; set; }
	}
}
