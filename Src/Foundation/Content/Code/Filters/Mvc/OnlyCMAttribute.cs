// <copyright file="ThrottleAttribute.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Filters.Mvc
{
    using global::Sitecore.Globalization;
    using Helpers;
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Mvc;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers;

    /// <summary>
    /// Decorates any MVC route that needs to have client requests limited by time.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class OnlyCMAttribute : ActionFilterAttribute
    {
        public string Message { get; set; } = Translate.Text("You are not allowed to view this page");

        /// <summary>
        /// The OnActionExecuting
        /// </summary>
        /// <param name="c">The c<see cref="ActionExecutingContext"/></param>
        public override void OnActionExecuting(ActionExecutingContext c)
        {
           var strcmcd = ConfigurationManager.AppSettings[ConfigKeys.CMorCD];
            if(!string.IsNullOrWhiteSpace(strcmcd) && strcmcd.ToLower().Equals("cm"))
            {

            }
            else
            {
                if (string.IsNullOrEmpty(Message))
                    Message = "You are not allowed to view this page";
                var redirect = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.ERROR_404);
                c.Result = new RedirectResult(redirect);
                return;
            }
        }
    }
}
