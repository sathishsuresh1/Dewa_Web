using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class SocialMediaFeedSet : SectionTitle
	{
		public virtual IEnumerable<SocialMediaFeed> Children { get; set; }
	}
}