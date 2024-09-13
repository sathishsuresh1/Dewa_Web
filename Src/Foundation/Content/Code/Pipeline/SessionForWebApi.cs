using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace DEWAXP.Foundation.Content.Pipeline
{
    public class SessionForWebApi: HttpRequestProcessor
    {
        private SessionStateBehavior behavior;
        public SessionForWebApi()
        {
            behavior = SessionStateBehavior.Required;
        }
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            HttpContext.Current.SetSessionStateBehavior(behavior);
        }
    }
}