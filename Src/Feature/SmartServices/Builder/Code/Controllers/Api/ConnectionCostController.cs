using DEWAXP.Foundation.Content.Controllers.Api;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Responses;
using DEWAXP.Foundation.Integration.Responses.ConsultantSvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;

namespace DEWAXP.Feature.Builder.Controllers.Api
{
    public class ConnectionCostController : BaseApiController
    {
        /// <summary>
        /// This method is used to get the data from web service
        /// </summary>
        /// <param name="el" desc="Existing Load"></param>
        /// <param name="pl" desc="Proposed Load"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string el, string pl, HttpRequestMessage request)
        {
            this.ValidateRequestHeader(request);
            ServiceResponse<ConnectionCalculatorResponse> connectionCalCharges = this.ConsultantServiceClient.GetConnectionCalCharges(el, pl, this.RequestLanguage, this.Request.Segment());

            if (connectionCalCharges.Succeeded && connectionCalCharges.Succeeded)
                return this.Request.CreateResponse<ConnectionCalculatorResponse>(HttpStatusCode.OK, connectionCalCharges.Payload);
            return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, connectionCalCharges.Message + connectionCalCharges.Message);
        }

        private void ValidateRequestHeader(HttpRequestMessage request)
        {
            string cookieToken = "";
            string formToken = "";

            IEnumerable<string> tokenHeaders;
            if (request.Headers.TryGetValues("RequestVerificationToken", out tokenHeaders))
            {
                string[] tokens = tokenHeaders.First().Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
                AntiForgery.Validate(cookieToken, formToken);
            }
        }
    }
}