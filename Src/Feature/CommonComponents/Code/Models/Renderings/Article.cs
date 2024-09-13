using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	[SitecoreType(TemplateName = "Article", TemplateId = "{B103022D-F6FD-4BD6-A878-2B05E011CED9}", AutoMap = true)]
	public class Article : PageBase, IM8Teaser
	{
		[SitecoreField("Rich Text")]
		public virtual string RichText { get; set; }

		[SitecoreField("Teaser Image")]
		public virtual Image TeaserImage { get; set; }

        [SitecoreField("NewsArticle")]
        public virtual string NewsArticle { get; set; }
	}

	[SitecoreType(TemplateName = "Article A", TemplateId = "{8793F755-854E-4CB8-93C3-043B6CEBC6FE}", AutoMap = true)]
	public class ArticleA : Article
	{
		
	}

	[SitecoreType(TemplateName = "Article B", TemplateId = "{B46C6F14-BBD5-41B7-83D5-2B40E46576C5}", AutoMap = true)]
	public class ArticleB : Article
	{

	}
}