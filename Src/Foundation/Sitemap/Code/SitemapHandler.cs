using DEWAXP.Foundation.Sitemap.Pipelines;
using Sitecore.Pipelines;
using Sitecore.Xml;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace DEWAXP.Foundation.Sitemap
{
    public sealed class SitemapHandler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            var configuredSites = Sitecore.Configuration.Factory.GetConfigNodes("sitemap/site")
                .Cast<XmlNode>()
                .Select(node => XmlUtil.GetAttribute("name", node));

            var website = Sitecore.Configuration.Factory.GetSiteInfoList()
                .FirstOrDefault(i => System.Convert.ToBoolean(i.HostName.ToLower()?.Contains(context.Request.Url.Host.ToLower())));

            if (website == null) //could not find by hostname, return "website"
                website = Sitecore.Configuration.Factory.GetSiteInfoList()
                .FirstOrDefault(i => i.Name.ToLowerInvariant() == "website");

            if (website == null || (website.Port > 0 && website.Port != context.Request.Url.Port))
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (!configuredSites.Contains(website.Name))
            {
                context.Response.StatusCode = 404;
                return;
            }

            Sitecore.Context.SetActiveSite(website.Name);

            context.Response.ContentType = "text/xml";
            context.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            var args = new CreateSitemapXmlArgs(website);
            CorePipeline.Run("createSitemapXml", args);
            var sb = new StringBuilder();

            foreach (var node in args.Nodes)
            {
                sb.Append("<url>")
                    .AppendFormat("<loc>{0}</loc>", node.Location?.Replace("http://","https://"))
                    .AppendFormat("<lastmod>{0}</lastmod>", node.LastModified.ToString("yyyy-MM-dd"))
                    .Append("</url>");
                context.Response.Write(sb.ToString());
                sb.Clear();
            }
            context.Response.Write("</urlset>");
        }
    }
}
