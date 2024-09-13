using System.Web;
using System.Web.Http.Controllers;

namespace DEWAXP.Foundation.Content.Filters.Http
{
    public class TwoPhaseAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (base.IsAuthorized(actionContext))
            {
                var header = HttpContext.Current.Request.Cookies;
                if (header.Count == 0) return false;

                if (HttpContext.Current.Session[GenericConstants.AntiHijackCookieName] != null)
                {
                    var antiHijackCookie = HttpContext.Current.Request.Cookies.Get(GenericConstants.AntiHijackCookieName);
                    var storedToken = System.Convert.ToString(HttpContext.Current?.Session?[GenericConstants.AntiHijackCookieName]);

                    if (antiHijackCookie == null || string.IsNullOrWhiteSpace(storedToken)) return false;

                    if (string.Equals(storedToken, antiHijackCookie.Value)) return true;
                }
            }
            return false;
        }
    }
}