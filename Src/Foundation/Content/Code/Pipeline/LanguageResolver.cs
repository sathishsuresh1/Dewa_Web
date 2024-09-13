using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;
using System;
using SitecoreContext = Sitecore.Context;

namespace DEWAXP.Foundation.Content.Pipelines
{
    public class DewaLanguageResolver : LanguageResolver
    {
        public override void Process(HttpRequestArgs args)
        {
            base.Process(args);

            var cookieName = SitecoreContext.Site.GetCookieKey("lang");
            var cookie = args.HttpContext.Request.Cookies[cookieName];
            if (cookie != null && cookie.Expires < DateTime.MaxValue)
            {
                WebUtil.SetCookieValue(cookieName, SitecoreContext.Language.Name, DateTime.MaxValue);
            }
        }
    }
}