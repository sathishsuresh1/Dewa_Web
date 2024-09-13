using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class LatestUpdateListing : Listing
	{
        public virtual IEnumerable<LatestUpdate> Children { get; set; }

        public virtual bool ShowFirstDetail { get; set; }
	}
}