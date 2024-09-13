using System.Collections.Generic;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data.Items;
namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class M60Carousel : SectionTitle
	{
		public virtual IEnumerable<M60CarouselSlide> Children { get; set; }
	}

	public class M60CarouselSlide : ContentBase
	{
		public virtual string Header { get; set; }
		public virtual string Subheader { get; set; }
		public virtual Image Image { get; set; }

		[SitecoreField("Button Text")]
		public virtual string ButtonText { get; set; }

		[SitecoreField("Button Link")]
		public virtual Item ButtonLink { get; set; }
	}
}