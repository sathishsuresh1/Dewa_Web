using DEWAXP.Foundation.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DEWAXP.Foundation.Integration.Requests.SmartVendor;
using DEWAXP.Foundation.Content.Controllers.Api;

namespace DEWAXP.Feature.GatePass.Controllers.Api.Common
{
    public class TradeLicenseDetailsController : BaseApiController
    {
        public async Task<HttpResponseMessage> Get(string tid)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(tid))
                    {
                        var response = SmartVendorClient.GetTradeLicenseDetails(
                            new TradeLicenseRequest
                            {
                                tradedetailsinput = new Tradedetailsinput
                                {
                                    tradelicensenumber = tid,
                                    sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                                    userid = CurrentPrincipal.UserId ?? string.Empty,
                                }
                            }, RequestLanguage, Request.Segment());
                        if (response != null && response.Succeeded && response.Payload != null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { result = response.Payload });
                        }
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = response.Message });
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage);
            }))());





        }
    }
}
