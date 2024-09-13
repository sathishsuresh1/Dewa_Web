using Sitecore.Pipelines;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Sitemap.Pipelines
{
    public class CreateSitemapXmlArgs : PipelineArgs
    {
        public Sitecore.Web.SiteInfo Site { get; private set; }
        public List<UrlDefinition> Nodes { get; private set; }

        public CreateSitemapXmlArgs(Sitecore.Web.SiteInfo site)
        {
            Nodes = new List<UrlDefinition>();
            Site = site;
        }
    }
}