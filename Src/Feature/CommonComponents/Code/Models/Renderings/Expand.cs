using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data.Items;
using Glass.Mapper.Sc.Fields;
using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class Expand : ContentBase
	{
		//public Expand()
		//{
		//	Children = new List<ExpandItem>();
		//}

		[SitecoreChildren]
		public virtual IEnumerable<ExpandItem> Children { get; set; }
	}
    [SitecoreType(TemplateId = "{A10F1F12-8F5C-443B-B72B-3CC5177C5557}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class ExpandItem : ContentBase
    {

        [SitecoreField("Title")]
        public virtual string ExpandTitle { get; set; }

        [SitecoreField("Content")]
        public virtual string Expandcontent { get; set; }
    }

}