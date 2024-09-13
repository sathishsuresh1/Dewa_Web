using System.Collections.Generic;
using Glass.Mapper.Sc.Fields;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data.Items;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class Hero : ContentBase
	{
		public virtual IEnumerable<HeroSlide> Children { get; set; }
	}

    [SitecoreType(TemplateName = "Hero Slide", TemplateId = "{CAF9A1B2-D231-4FE1-8259-BF95301A9BE4}", AutoMap = true)]
    public class HeroSlide : ContentBase
	{
		public virtual string Header { get; set; }
		public virtual string Subheader { get; set; }
		public virtual string Description { get; set; }
		public virtual Image Image { get; set; }
        
        [SitecoreField("Redirect Text")]
        public virtual string RedirectText { get; set; }

        [SitecoreField("Redirect Link")]
        public virtual Item RedirectLink { get; set; }

       // [SitecoreField("Redirect Link")]
        public virtual Link DownloadLink { get; set; }
    }
    [SitecoreType(TemplateId = "{C3A1C124-4240-4998-9AA7-A1176B006921}", AutoMap = true)]
    public class DataSourceTemplateValue : ContentBase
    {
        [SitecoreField("Value")]
        public virtual string value { get; set; }
    }
}