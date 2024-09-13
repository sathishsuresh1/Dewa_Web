using Glass.Mapper.Sc.Configuration.Attributes;

namespace DEWAXP.Foundation.ORM.Models.Sitemap
{
	[SitecoreType(TemplateId = "{19728952-F2A2-47E9-989B-67FF66081C3F}", AutoMap = true)]
	public interface ISitemap
	{
	    bool HideFromSitemapPage { get; set; }
	}
}