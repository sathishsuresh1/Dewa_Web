using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    [SitecoreType( TemplateId = "{D4E2BD63-CC54-4FEB-A821-7FF36425CEB2}", AutoMap = true)]
	public class LatestUpdate : PageBase, IM8Teaser
	{
        [SitecoreField("LatestUpdate")]
        public virtual string LatestUpdateArticle { get; set; }

		[SitecoreField("Teaser Image")]
		public virtual Image TeaserImage { get; set; }

        
	}

	
}