// <copyright file="EpassController.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Controllers.Api
{
    using DEWAXP.Feature.GatePass.Models;
    using DEWAXP.Feature.GatePass.Models.ePass;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="EpassController" />
    /// </summary>
    public class EpassController : BaseApiController
    {
        /// <summary>
        /// The Getpassedrecords
        /// </summary>
        /// <returns>The <see cref="HttpResponseMessage"/></returns>
        [HttpPost]
        public HttpResponseMessage PassedLongtermrecords()
        {
            List<CSVfileformat> model = new List<CSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.EPASS_MULTIPASS_LIST, out model))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, Result = model });
            }
            HttpContext.Current.Response.Redirect(RedirectUrl(SitecoreItemIdentifiers.EPASS_PERMANENT_PASS));
            return null;
        }
    }
}
