using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;
using System;
using System.Web;

namespace DEWAXP.Foundation.Content.Pipelines
{
    public class CookieRequestHandler : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            string _serivceCenter = WebUtil.GetQueryString("srv");
            string _branch = WebUtil.GetQueryString("sc");
            string _futureCenterValues = _branch + "|" + _serivceCenter;

            if (!string.IsNullOrEmpty(_branch))
            {
                //future_digital_centers
                HttpCookie cookie = new HttpCookie("fdc");
                cookie.Expires = DateTime.Now.AddYears(100);
                cookie.Value = _futureCenterValues;

                if (HttpContext.Current.Request.Cookies["fdc"] == null)
                {
                    HttpContext.Current.Response.Cookies.Remove("fdc");
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
                else
                {
                    // Set the cookies and allow the device to configure multiple times.
                    HttpContext.Current.Response.Cookies.Set(cookie);
                }

                HttpContext.Current.Response.Redirect("~/customer/my-account/login");
            }
        }
    }
}