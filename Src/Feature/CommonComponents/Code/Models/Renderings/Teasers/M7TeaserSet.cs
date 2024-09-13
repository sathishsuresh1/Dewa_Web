using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
	public class M7TeaserSet : SectionTitle
	{
		public virtual IEnumerable<M7Teaser> Children { get; set; }
	}
}