// <copyright file="ThrottleAttribute.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Filters.Mvc
{
    using global::Sitecore.Globalization;
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Mvc;

    /// <summary>
    /// Decorates any MVC route that needs to have client requests limited by time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the Name
        /// A unique name for this Throttle.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the Minutes
        /// The number of Minutes clients must wait before executing this decorated route again.
        /// </summary>
        private int Minutes { get; } = int.Parse(ConfigurationManager.AppSettings.Get("THROTTLEMILLISECONDS"));

        /// <summary>
        /// Gets the Count
        /// The number of count clients must wait before executing this decorated route again.
        /// </summary>
        public int Count { get; } = int.Parse(ConfigurationManager.AppSettings.Get("THROTTLECOUNT"));

        /// <summary>
        /// Gets or sets the Message
        /// A text message that will be sent to the client upon throttling.  You can include the token {n} to
        /// show this.Minutes in the message, e.g. "Wait {n} Minutes before trying again".
        /// </summary>
        public string Message { get; set; } = Translate.Text("Throttle message");

        /// <summary>
        /// The OnActionExecuting
        /// </summary>
        /// <param name="c">The c<see cref="ActionExecutingContext"/></param>
        public override void OnActionExecuting(ActionExecutingContext c)
        {
            var key = string.Concat(Name, "-", c.HttpContext.Request.UserHostAddress);
            var allowExecute = false;
            if (HttpRuntime.Cache[key] == null)
            {
                HttpRuntime.Cache.Add(key,
                    1, // is this the smallest data we can have?
                    null, // no dependencies
                    DateTime.Now.AddMinutes(Minutes), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback

                allowExecute = true;
            }
            else
            {
                int keycount = (int)HttpRuntime.Cache.Get(key);
                if (keycount < Count)
                {
                    HttpRuntime.Cache.Remove(key);
                    HttpRuntime.Cache.Add(key,
                    keycount + 1, // is this the smallest data we can have?
                    null, // no dependencies
                    DateTime.Now.AddMinutes(Minutes), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback

                    allowExecute = true;
                }
            }

            if (!allowExecute)
            {
                if (string.IsNullOrEmpty(Message))
                    Message = "You may only perform this action every {n} Minutes.";
                c.Result = new HttpStatusCodeResult((System.Net.HttpStatusCode)429, Message.Replace("{n}", Minutes.ToString()));
                return;
            }
        }
    }
}
