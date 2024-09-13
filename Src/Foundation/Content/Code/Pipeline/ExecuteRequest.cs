using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Models.Outage;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Web;
using System;
using System.Web;

namespace DEWAXP.Foundation.Content.Pipelines
{
    public class ExecuteRequest : global::Sitecore.Pipelines.HttpRequest.ExecuteRequest
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));

        public ExecuteRequest(BaseSiteManager siteManager, BaseItemManager itemManager) : base(siteManager, itemManager)
        {
        }

        protected override void RedirectOnItemNotFound(string url)
        {
            var context = System.Web.HttpContext.Current;

            try
            {
                var itemUrl = System.Web.HttpUtility.UrlDecode(url); //decode url to remove funny characters

                //if legacy news page, redirect to landing.
                if (!string.IsNullOrEmpty(itemUrl) && itemUrl.Contains("/news/details"))
                {
                    //redirect to landing
                    var newsLanding = _contentRepository.GetItem<GlassBase>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.NEWS_LANDING_PAGE)));
                    if (newsLanding != null && newsLanding.Id != Guid.Empty)
                    {
                        HttpContext current = HttpContext.Current;
                        if (current != null)
                        {
                            var response = current.Response;
                            response.StatusCode = 301;
                            response.StatusDescription = "Moved Permanently";
                            response.RedirectLocation = newsLanding.Url;
                            response.Flush();
                        }
                    }
                }

                // Request the NotFound page
                var pageNotFoundItem = Context.Database.GetItem(SitecoreItemIdentifiers.ERROR_404);
                var contentUrl = LinkManager.GetItemUrl(pageNotFoundItem, new ItemUrlBuilderOptions()
                {
                    AlwaysIncludeServerUrl = true,
                    LanguageEmbedding = LanguageEmbedding.Always,
                    AddAspxExtension = false,
                });

                string urlParam = url.Substring(url.IndexOf('?'), url.Length - url.IndexOf('?'));
                //var content = global::Sitecore.Web.WebUtil.ExecuteWebPage(contentUrl);
                global::Sitecore.Web.WebUtil.Redirect(contentUrl + urlParam, false);

                // Send the NotFound page content to the client with a 404 status code
                context.Response.TrySkipIisCustomErrors = true;
                context.Response.StatusCode = 404;
                //context.Response.ContentType = "text/html";
                //context.Response.Write(content);
            }
            catch (Exception)
            {
                // If our plan fails for any reason, fall back to the base method
                base.RedirectOnItemNotFound(url);
            }
            // Must be outside the try/catch, cause Response.End() throws an exception
            context.Response.End();
        }

        protected override void PerformRedirect(string url)
        {
            var outageconfig = _contentRepository.GetItem<OutageConfigModel>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.OUTAGECONFIG)));
            if (outageconfig != null && outageconfig.TurnOnOutage)
            {
                var outagepage = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.OUTAGEHOMEPAGE);
                if (!string.IsNullOrEmpty(outagepage))
                {
                    WebUtil.Redirect(outagepage, false);
                    System.Web.HttpContext.Current.Response.End();
                }
            }
        }
    }
}