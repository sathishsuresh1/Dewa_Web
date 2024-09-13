using System;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
	public interface IM8Teaser
	{
		string Header { get; set; }
		string Summary { get; set; }
		DateTime PublishDate { get; set; }
		Image TeaserImage { get; set; }

		string Url { get; set; }
	}
}