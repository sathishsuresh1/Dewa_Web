using DEWAXP.Foundation.Content.Models.Outage;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Data.Items;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;
using System;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Pipeline
{
    public class OutageRedirectHandler : HttpRequestProcessor
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        private static IContextRepository _contextRepository = new ContextRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        public override void Process(HttpRequestArgs args)
        {
            var currentItem = _contextRepository.GetCurrentItem<Item>();
            var currentItembase = _contextRepository.GetCurrentItem<PageBase>();
            if (currentItem == null || currentItem.Parent == null) return;
            if (currentItembase==null|| currentItembase.WebService == null) return;
            if (currentItem.ID.Guid == Guid.Parse(SitecoreItemIdentifiers.OUTAGEHOMEPAGE) ||
                currentItem.ID.Guid == Guid.Parse(SitecoreItemIdentifiers.LATESTUPDATESPAGE) ||
                currentItem.Parent.ID.Guid == Guid.Parse(SitecoreItemIdentifiers.LATESTUPDATESPAGE))
            {
                return;
            }

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