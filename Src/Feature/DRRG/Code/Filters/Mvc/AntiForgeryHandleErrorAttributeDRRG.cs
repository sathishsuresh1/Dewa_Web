using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;
using DEWAXP.Feature.DRRG.Controllers;
using DEWAXP.Feature.DRRG.Models;

namespace DEWAXP.Feature.DRRG.Filters.Mvc
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
       Justification = "This attribute is AllowMultiple = true and users might want to override behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AntiForgeryHandleErrorAttributeDRRG : ActionFilterAttribute, IExceptionFilter
    {
        public virtual void OnException(ExceptionContext context)
        {
            if (context.Exception is HttpAntiForgeryException)
            {
                var url = string.Empty;
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var requestContext = new RequestContext(context.HttpContext, context.RouteData);
                    url = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_HOME);
                }
                else
                {
                    url = GetRedirectUrl(context);
                    var controller = context.Controller as DRRGRegistrationController;
                    controller.LogoutUser(context.Controller?.ViewData?.Model as LoginModel);
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
                var url = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_HOME);

                return url;
            }
            catch (Exception)
            {
                throw new NullReferenceException();
            }
        }
    }
}