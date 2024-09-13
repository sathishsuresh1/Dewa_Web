using DEWAXP.Feature.ChatBot.Controllers;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;

namespace DEWAXP.Feature.ChatBot.Filters
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "This attribute is AllowMultiple = true and users might want to override behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AntiForgeryHandleErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public virtual void OnException(ExceptionContext context)
        {
            if (context.Exception is HttpAntiForgeryException)
            {
                var url = string.Empty;
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var requestContext = new RequestContext(context.HttpContext, context.RouteData);
                    url = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);
                }
                else
                {
                    url = GetRedirectUrl(context);
                    var controller = context.Controller as RammasController;
                    controller.ClearSessionAndSignOut();
                    context.HttpContext.Response.StatusCode = 200;
                    context.ExceptionHandled = true;
                }
                context.HttpContext.Response.Redirect(url, true);
            }
            else
            {
                return;
            }
        }

        private string GetRedirectUrl(ExceptionContext context)
        {
            try
            {
                var requestContext = new RequestContext(context.HttpContext, context.RouteData);
                var url = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J7_LOGIN_PAGE);

                return url;
            }
            catch (Exception)
            {
                throw new NullReferenceException();
            }
        }
    }
}