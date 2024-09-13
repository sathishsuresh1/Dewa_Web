using System.Collections.Generic;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
	[SitecoreType(TemplateName = "Home", TemplateId = "{816DAADA-314D-4314-98D5-B52957B99BAB}", AutoMap = true)]
	public class Home : ContentBase
	{
		[SitecoreChildren(InferType = true)]
		public virtual IEnumerable<ContentBase> Children { get; set; }
	}
}