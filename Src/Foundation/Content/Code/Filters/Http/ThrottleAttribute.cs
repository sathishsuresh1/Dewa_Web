// <copyright file="ThrottleAttribute.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Filters.Http
{
    using global::Sitecore.Globalization;
    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

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
        /// Gets the minutes
        /// The number of minutes clients must wait before executing this decorated route again.
        /// </summary>
        private int minutes { get; } = int.Parse(ConfigurationManager.AppSettings.Get("THROTTLEMILLISECONDS"));

        /// <summary>
        /// Gets the Count
        /// The number of count clients must wait before executing this decorated route again.
        /// </summary>
        public int Count { get; } = int.Parse(ConfigurationManager.AppSettings.Get("THROTTLECOUNT"));

        /// <summary>
        /// Gets or sets the Message
        /// A text message that will be sent to the client upon throttling.  You can include the token {n} to
        /// show this.minutes in the message, e.g. "Wait {n} minutes before trying again".
        /// </summary>
        public string Message { get; set; } = Translate.Text("Throttle message");

        /// <summary>
        /// The OnActionExecuting
        /// </summary>
        /// <param name="c">The c<see cref="HttpActionContext"/></param>
        public override void OnActionExecuting(HttpActionContext c)
        {
            var key = string.Concat(Name, "-", GetClientIp(c.Request));
            var allowExecute = false;
            if (HttpRuntime.Cache[key] == null)
            {
                HttpRuntime.Cache.Add(key,
                    1, // is this the smallest data we can have?
                    null, // no dependencies
                    DateTime.Now.AddMinutes(minutes), // absolute expiration
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
                    DateTime.Now.AddMinutes(minutes), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback

                    allowExecute = true;
                }
            }

            if (!allowExecute)
            {
                if (string.IsNullOrEmpty(Message))
                    Message = "You may only perform this action every {n} minutes.";
                c.Response = new HttpResponseMessage((HttpStatusCode)429) { ReasonPhrase = Message.Replace("{n}", minutes.ToString()) };
            }
        }

        /// <summary>
        /// The GetClientIp
        /// </summary>
        /// <param name="request">The request<see cref="HttpRequestMessage"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return null;
        }
    }
}
