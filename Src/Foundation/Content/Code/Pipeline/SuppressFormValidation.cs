using Sitecore.Configuration;
using Sitecore.Pipelines.PreprocessRequest;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWA.Website.Pipelines
{
    public class SuppressFormValidation : global::Sitecore.Pipelines.PreprocessRequest.SuppressFormValidation
    {

        private static readonly List<string> sitesToIgnoreByCustomProcessors = Settings.GetSetting("SitesToIgnoreByCustomProcessors", string.Empty).ToLowerInvariant().Split(new char[1]
        {
        ','
        }, StringSplitOptions.RemoveEmptyEntries).ToList();

        public override void Process(PreprocessRequestArgs args)
        {
            var sitecoreContext =
            SiteContextFactory.GetSiteContext(args.Context.Request.Url.Host, args.Context.Request.Url.PathAndQuery);
            if (sitecoreContext==null)
            {
                return;
            }

            if (sitesToIgnoreByCustomProcessors.Contains(sitecoreContext.Name.ToLowerInvariant()))
            {
                return;
            }

            base.Process(args);
        }
    }
}