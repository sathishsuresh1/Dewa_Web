using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data.Items;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class Map : ContentBase
	{
		[SitecoreField("Input Label")]
		public virtual string InputLabel { get; set; }

		[SitecoreField("Input Placeholder")]
		public virtual string InputPlaceholder { get; set; }

        [SitecoreField("Map Type")]
        public virtual string MapType { get; set; }

        [SitecoreField("Show Search")]
        public virtual bool ShowSearch { get; set; }
        

		
	}
}
