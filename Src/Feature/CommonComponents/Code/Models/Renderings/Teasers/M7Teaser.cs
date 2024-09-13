using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
	public class M7Teaser : Teaser
	{
		public virtual string Featured { get; set; }

		[SitecoreField("Link Text")]
		public virtual string LinkText { get; set; }
		//      public virtual Image Image { get; set; }
	}
}