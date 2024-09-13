using DEWAXP.Foundation.Content.Models.Outage;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.ORM.Models;
using DEWAXP.Foundation.ORM.Models.Outage;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Fields;
using Glass.Mapper.Sc.Web;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Pipeline
{
    public class WebserviceOutageRedirectHandler : HttpRequestProcessor
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        private static IContextRepository _contextRepository = new ContextRepository(new RequestContext(new SitecoreService(Sitecore.Context.Database)));
        public override void Process(HttpRequestArgs args)
        {
            var currentItem = _contextRepository.GetCurrentItem<PageBase>();
            
            if (currentItem != null)
            {
                var outageconfig = _contentRepository.GetItem<OutageConfigModel>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.OUTAGECONFIG)));
                var outagepage = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.WEBSERVICEOUTAGEPAGE);
                if (currentItem.Id == Guid.Parse(SitecoreItemIdentifiers.WEBSERVICEOUTAGEPAGE))
                {
                    if (outageconfig != null && !string.IsNullOrEmpty(outagepage) && outageconfig.WebserviceOutage != null && outageconfig.WebserviceOutage.Count() > 0)
                    {
                        var OutageList = outageconfig.WebserviceOutage.Where(x => x != null && this.IsWebservicedown(x)).Any();
                        if (OutageList)
                        {
                            return;
                        }
                    }
                    var homepage = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.HOME);
                    WebUtil.Redirect(homepage, false);
                    HttpContext.Current.Response.End();
                }
                if (currentItem.WebService != null && currentItem.WebService.Count() > 0)
                {
                    
                    if (currentItem.WebService.FirstOrDefault().OutageURL != null && !string.IsNullOrWhiteSpace(currentItem.WebService.FirstOrDefault().OutageURL.Url))
                    {
                        if (currentItem.WebService.FirstOrDefault().OutageURL.Type.Equals(LinkType.Internal) && currentItem.WebService.FirstOrDefault().OutageURL.TargetId != null)
                        {
                            outagepage = LinkHelper.GetItemUrl(currentItem.WebService.FirstOrDefault().OutageURL.TargetId.ToString());
                        }
                    }
                    if (outageconfig != null && !string.IsNullOrEmpty(outagepage) && outageconfig.WebserviceOutage != null && outageconfig.WebserviceOutage.Count() > 0)
                    {
                        var OutageList = outageconfig.WebserviceOutage.Where(x => x != null && this.IsWebservicedown(x) && currentItem.WebService.Any(y => y.Id.Equals(x.Id))).Any();
                        if (OutageList)
                        {
                            WebUtil.Redirect(outagepage, false);
                            HttpContext.Current.Response.End();
                        }
                    }
                }
            }
            return;
        }

        private bool IsWebservicedown(OutageItem model)
        {
            if (model.StartDate != null && model.EndDate != null)
            {
                return (DateTime.Compare(DateTime.Now, model.StartDate.Value) >= 0 &&
                        DateTime.Compare(DateTime.Now, model.EndDate.Value) <= 0);
            }
            return false;
        }
    }
}