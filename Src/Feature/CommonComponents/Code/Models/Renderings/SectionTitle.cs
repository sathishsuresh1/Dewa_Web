using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data.Items;
using Glass.Mapper.Sc.Fields;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class SectionTitle : ContentBase
	{
		public virtual string Header { get; set; }
        [SitecoreField("Description")]
        public virtual string Description { get; set; }

        [SitecoreField("Redirect Text")]
		public virtual string RedirectText { get; set; }

		[SitecoreField("Redirect Link")]
		public virtual Item RedirectLink { get; set; }

		[SitecoreField("Button Text")]
		public virtual string ButtonText { get; set; }

		[SitecoreField("Button Link")]
        public virtual Item ButtonLink { get; set; }
	}
}
