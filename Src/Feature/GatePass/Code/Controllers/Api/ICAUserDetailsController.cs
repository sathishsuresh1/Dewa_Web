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
    public class ICAUserDetailsController : BaseApiController
    {
        public async Task<HttpResponseMessage> Get(string eid, string eidexp,string pcat,string pcode, string pno, string psrc,bool ica)
        {
            string ErrorMessage = "";
            return await Task.FromResult(((Func<HttpResponseMessage>)(() =>
            {
                try
                {
                    var response = SmartVendorClient.GetICADetails(
                        new UserDetailsRequest
                        {
                            inquiryinput = new Inquiryinput
                            {
                                icainquirydetails = new Icainquirydetails
                                {
                                    emiratesid = eid,
                                    emiratesidexpirydate = converttodate(eidexp).HasValue ? converttodate(eidexp).Value.ToString("yyyyMMdd") : string.Empty,
                                },
                                rtainquirydetails = new Rtainquirydetails
                                {
                                    platecategory = pcat,
                                    platecode =pcode,
                                    platenumber =pno,
                                    platesource = psrc,
                                    registrationstatus = "Registered"
                                },
                                sessionid = CurrentPrincipal.SessionToken??string.Empty,
                                userid = CurrentPrincipal.UserId??string.Empty,
                                icartaflag= ica?"I":"R"
                            }
                        }, RequestLanguage, Request.Segment());
                    if (response != null && response.Succeeded && response.Payload != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { result = response.Payload });
                    }
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = response.Message });
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
