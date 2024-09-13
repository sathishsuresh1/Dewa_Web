using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data.Items;
using Glass.Mapper.Sc.Fields;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType(TemplateId = "{624DB709-56F3-40DF-A9E6-65DF20A1B0F2}", AutoMap = true)]
    public class M69HelpandSupport : ContentBase
	{
        [SitecoreField("Header")]
        public virtual string Header { get; set; }
        [SitecoreField("Subheader")]
        public virtual string Subheader { get; set; }

        [SitecoreField("Image")]
		public virtual Image Image { get; set; }

		[SitecoreField("Button Text")]
		public virtual string ButtonText { get; set; }

		[SitecoreField("Button Link")]
        public virtual Link ButtonLink { get; set; }
	}
}
