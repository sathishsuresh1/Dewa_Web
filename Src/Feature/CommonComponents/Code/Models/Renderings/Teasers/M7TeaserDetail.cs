using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers
{
    [SitecoreType(TemplateId ="{34A6F02A-F5D9-4121-9A22-06B2C11EF30B}",AutoMap = true)]
	public class M7TeaserDetail : GlassBase
    {
		public virtual string Title { get; set; }

		[SitecoreField("Host Detail")]
		public virtual string HostDetail { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual Image Image { get; set; }
    }
}