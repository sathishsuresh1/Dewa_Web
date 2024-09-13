// <copyright file="WorkPermitController.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GatePass.Controllers.Api
{
    using DEWAXP.Feature.GatePass.Models.WorkPermit;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers.Api;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Defines the <see cref="WorkPermitController" />.
    /// </summary>
    public class WorkPermitController : BaseApiController
    {
        /// <summary>
        /// The PassedWorkPermitrecords.
        /// </summary>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        [HttpPost]
        public HttpResponseMessage PassedWorkPermitrecords()
        {
            List<WorkPermitCSVfileformat> model = new List<WorkPermitCSVfileformat>();
            if (CacheProvider.TryGet(CacheKeys.WORK_PERMIT_MULTIPASS_LIST, out model))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, Result = model });
            }
            HttpContext.Current.Response.Redirect(RedirectUrl(SitecoreItemIdentifiers.EPASS_PERMANENT_PASS));
            return null;
        }
    }
}
