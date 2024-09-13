using DEWAXP.Foundation.Content.Provider;
using DEWAXP.Foundation.Content.Utils;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Sites;
using Sitecore.Web;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Handlers.Robots
{
    public class RobotsHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var site = GetSiteContext(HttpContext.Current.Request.Url);
            if (site != null)
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain";
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                IRobotsTxtProvider robotsTxtProvider = DependencyResolver.Current.GetService<IRobotsTxtProvider>();
                context.Response.Write(robotsTxtProvider.GetRobotsTxtFileContent(site));
            }
            context.Response.End();
        }
        public static SiteContext GetSiteContext(Uri requestUrl)
        {
            Assert.ArgumentNotNull(requestUrl, "requestUrl");
            string requestHostName = requestUrl.Host;

            foreach (SiteInfo siteInfo in Factory.GetSiteInfoList())
            {
                if (RegexUtil.IsWildcardMatch(requestHostName, siteInfo.HostName) || RegexUtil.IsWildcardMatch(requestHostName, siteInfo.TargetHostName))
                {
                    return new SiteContext(siteInfo);
                }
            }
            return SiteContext.GetSite("website");
        }
    }
}