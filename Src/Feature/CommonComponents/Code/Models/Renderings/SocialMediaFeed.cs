using System;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	[SitecoreType(TemplateId = "{161F45C6-2B1A-4364-A4CD-AE6AAD393E89}")]
	public class SocialMediaFeed : ContentBase
	{
		[SitecoreField("Timestamp")]
		public DateTime Timestamp { get; set; }

		[SitecoreField("Text")]
		public virtual string Text { get; set; }

		[SitecoreField("Image")]
		public virtual Image Image { get; set; }

		[SitecoreField("Type")]
		public SocialMediaType Type { get; set; }

		public virtual string IconClass
		{
			get
			{
				if (Type == null) return "";
				return Type.Name.ToLower();
			}
		}
	}

	[SitecoreType(TemplateId = "{0D37D096-81B3-4198-815C-6157D402441F}")]
	public class SocialMediaType : ContentBase
	{
		[SitecoreField("Link")]
		public virtual string Link { get; set; }
	}

	public static class SocialMediaTypes
	{
		public const string Facebook = "Facebook";
		public const string Instagram = "Instagram";
		public const string YouTube = "YouTube";
		public const string Twitter = "Twitter";
	}
}