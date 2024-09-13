using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using DEWAXP.Feature.CommonComponents.Models.Renderings;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
	[SitecoreType(TemplateId = "{0B9C9818-969A-42A4-AF17-F86C837F51DA}", AutoMap = true)]
	public class MainLanding : PageBase
	{
		[SitecoreChildren(InferType = true)]
		public virtual IEnumerable<ContentBase> Children { get; set; }
	}
}