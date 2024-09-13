using DEWAXP.Foundation.Content.Models.Redirect;
using DEWAXP.Foundation.Content.Repositories;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Data.Items;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;
using System.Web;

namespace DEWAXP.Foundation.Content.Pipelines
{
    public class RedirectionLink : HttpRequestProcessor
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        private static IContextRepository _contextRepository = new ContextRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));

        public override void Process(HttpRequestArgs args)
        {
            var currentItem = _contextRepository.GetCurrentItem<Item>();

            if (currentItem != null && currentItem.TemplateID.ToString().Equals("{954B7389-A0CF-4512-BFB3-6D6DF1BF6003}"))
            {
                var redirectItem = _contentRepository.GetItem<RedirectPage>(new GetItemByItemOptions(currentItem));
                if (redirectItem.RedirectLink != null)
                {
                    var url = redirectItem.RedirectLink.Url;
                    WebUtil.Redirect(url, false);
                    HttpContext.Current.Response.End();
                }
            }
        }
    }
}