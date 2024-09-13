using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
	[SitecoreType(TemplateId = "{B3063DA8-14FF-4636-B08E-6F325A945972}", AutoMap = true)]
	public class M9TeaserSet : SectionTitle
	{
		[SitecoreField(FieldName = "Related Articles")]
		public virtual IEnumerable<M9Teaser> RelatedArticles { get; set; }
	}
}