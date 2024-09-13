using System;

namespace DEWAXP.Feature.SupplyManagement.Models.Complaints
{
	[Serializable]
	public class ReferenceNumber
	{
		public ReferenceNumber(string value, string ctaUrl, string ctaText)
		{
			Value = value;
			CtaUrl = ctaUrl;
			CtaText = ctaText;
		}

		public string Value { get; private set; }
		public string CtaUrl { get; private set; }
		public string CtaText { get; private set; }
	}
}