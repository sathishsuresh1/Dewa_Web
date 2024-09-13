using DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	[SitecoreType(TemplateId = "{960E0516-38BC-495D-A49B-57A9EB0CE1CA}", AutoMap = true)]
	public class NewsArticle : Article
	{
        public virtual string EFOLDERID { get; set; }
        public virtual string DMSID { get; set; }

        [SitecoreField("NewsArticle", Setting = SitecoreFieldSettings.RichTextRaw)]
        public virtual string NewsArticleBody { get; set; }
    }
}