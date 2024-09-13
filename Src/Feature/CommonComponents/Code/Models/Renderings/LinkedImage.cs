using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models
{
	[SitecoreType(TemplateId = "{CDB80949-7B39-45A9-B932-7280EDAC2CB0}")]
	public class LinkedImage : ContentBase
	{
		[SitecoreField("Image")]
		public virtual Image Image { get; set; }

		[SitecoreField("Link")]
		public virtual Link Link { get; set; }
	}
}