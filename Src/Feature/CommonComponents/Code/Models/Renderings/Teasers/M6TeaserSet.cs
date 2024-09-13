using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
	public class M6TeaserSet : SectionTitle
	{
		[SitecoreChildren]
		public virtual IEnumerable<Teaser> Children { get; set; }
	}
}