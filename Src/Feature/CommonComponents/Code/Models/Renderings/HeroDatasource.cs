using System.Collections.Generic;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class HeroDatasource : GlassBase
	{
        [SitecoreField("Hero List")]
		public virtual IEnumerable<HeroSlide> HeroList { get; set; }
        [SitecoreField("Number of Hero")]
        public virtual DataSourceTemplateValue numberofhero { get; set; }
	}

}